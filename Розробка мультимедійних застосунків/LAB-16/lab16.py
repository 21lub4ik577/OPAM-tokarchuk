"""
ЛАБОРАТОРНА РОБОТА 16 — ffmpeg і вимірювання              ЗАГОТОВКА
Розробка мультимедійних застосунків · КН-3/1 · 2026/2027

Готові: створення еталонного кліпу, запуск зовнішніх програм,
розбір виводу ffprobe, побудова графіків.

Вам треба заповнити ТРИ місця, позначені як # TODO.

Перевірка:   python lab16.py
Потрібні:    ffmpeg і ffprobe у PATH.   Перевірка: ffmpeg -version
"""

import os
import re
import subprocess
import time
import numpy as np
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
from PIL import Image
from skimage import data

W, H, FPS, NF = 1280, 720, 30, 150      # еталонний кліп: 5 секунд
WORK = "work"                            # сюди лягають усі файли
SRC = os.path.join(WORK, "src.mkv")      # джерело без втрат


# ════════════════════════════════════════════════════════════════════
#  ГОТОВЕ
# ════════════════════════════════════════════════════════════════════

def run(cmd):
    """Запускає програму й повертає результат. Нічого не друкує."""
    return subprocess.run(cmd, capture_output=True, text=True)


def make_source():
    """Будує еталонний кліп: панорама по колажу з різких фотографій.

    Колаж, а не одне фото, — щоб у кадрі були і дрібні деталі, і рівні
    ділянки. Кліп зберігається кодеком ffv1 БЕЗ ВТРАТ: саме з ним
    порівнюватимемо все інше, тому він мусить бути точним.
    """
    os.makedirs(WORK, exist_ok=True)
    if os.path.exists(SRC):
        print("   еталонний кліп уже є:", SRC)
        return
    tiles = []
    for name in ["astronaut", "chelsea", "coffee", "immunohistochemistry", "retina", "hubble_deep_field"]:
        a = getattr(data, name)()
        if a.ndim == 2:
            a = np.stack([a] * 3, -1)
        if a.dtype != np.uint8:
            a = (255 * (a - a.min()) / (a.ptp() + 1e-9)).astype(np.uint8)
        tiles.append(a[..., :3])

    CW, CH, CELL = 2400, 900, 300
    rc = np.random.default_rng(2)
    canvas = np.zeros((CH, CW, 3), np.uint8)
    for gy in range(CH // CELL):
        for gx in range(CW // CELL):
            t = tiles[rc.integers(0, len(tiles))]
            h, w = t.shape[:2]
            y0 = int(rc.integers(0, max(1, h - CELL + 1)))
            x0 = int(rc.integers(0, max(1, w - CELL + 1)))
            c = t[y0:y0 + CELL, x0:x0 + CELL]
            if c.shape[0] < CELL or c.shape[1] < CELL:
                c = np.array(Image.fromarray(t).resize((CELL, CELL)))
            canvas[gy * CELL:(gy + 1) * CELL, gx * CELL:(gx + 1) * CELL] = c

    rng = np.random.default_rng(7)
    raw = os.path.join(WORK, "src.raw")
    with open(raw, "wb") as fh:
        for i in range(NF):
            ox, oy = int(i * 6.2), int(20 * np.sin(i / 25))
            f = canvas[60 + oy:60 + oy + H, ox:ox + W].astype(np.int16)
            px = int(80 + i * 7.1) % (W - 120)
            py = int(300 + 160 * np.sin(i / 18))
            f[py:py + 110, px:px + 110] = [235, 205, 60]
            f = f + rng.normal(0, 2.0, f.shape)      # легке зерно
            fh.write(np.clip(f, 0, 255).astype(np.uint8).tobytes())
    r = run(["ffmpeg", "-y", "-hide_banner", "-loglevel", "error",
            "-f", "rawvideo", "-pix_fmt", "rgb24", "-s", f"{W}x{H}",
            "-r", str(FPS), "-i", raw, "-c:v", "ffv1",
            "-pix_fmt", "yuv420p", SRC])
    if r.returncode:
        raise RuntimeError("ffmpeg не зміг зібрати джерело:\n" + r.stderr[-500:])
    os.remove(raw)
    print(f"   зібрано {SRC}: {os.path.getsize(SRC)/2**20:.0f} МБ без втрат")


def probe(path):
    """Повертає словник із властивостями відеодоріжки."""
    keys = "codec_name,profile,width,height,r_frame_rate,pix_fmt,bit_rate,nb_frames"
    r = run(["ffprobe", "-v", "error", "-select_streams", "v:0", "-show_entries", f"stream={keys}", "-of", "default=nw=1", path])
    d = dict(l.split("=", 1) for l in r.stdout.strip().split("\n") if "=" in l)
    r = run(["ffprobe", "-v", "error", "-show_entries", "format=format_name,duration,size", "-of", "default=nw=1", path])
    d.update(dict(l.split("=", 1) for l in r.stdout.strip().split("\n") if "=" in l))
    return d


# Фільтр для порівняння. Три речі в ньому обов'язкові, і про кожну
# написано в методичці; прибрати хоч одну — і числа стануть беззмістовними.
CMP = ("[0:v]setpts=N/{fps}/TB,format=yuv420p,setparams=range=tv[a];" "[1:v]setpts=N/{fps}/TB,format=yuv420p,setparams=range=tv[b];[a][b]")


def plot_rd(rows, name="rd.png"):
    """Крива «розмір проти якості»."""
    fig, ax = plt.subplots(1, 2, figsize=(11, 3.8))
    kb = [r[1] / 1024 for r in rows]
    ax[0].plot([r[0] for r in rows], kb, "o-", color="#B35A1F")
    ax[0].set_xlabel("crf"); ax[0].set_ylabel("розмір, КБ"); ax[0].set_yscale("log")
    ax[1].plot(kb, [r[2] for r in rows], "o-", color="#1F6F66")
    ax[1].set_xlabel("розмір, КБ"); ax[1].set_ylabel("PSNR, дБ"); ax[1].set_xscale("log")
    for a in ax: a.grid(alpha=0.3)
    fig.tight_layout(); fig.savefig(name, dpi=130); plt.close(fig)
    print("   збережено:", name)


def plot_codecs(job_rows, name="codecs_rd.png"):
    """Криві розмір-проти-якість для різних відеокодеків."""
    fig, ax = plt.subplots(figsize=(8, 5))
    colors = {"H.264": "#B35A1F", "H.265": "#1F6F66", "AV1": "#7A5AA6"}
    for codec_name, rows in job_rows:
        sizes_kb = [r[2] / 1024 for r in rows]
        psnrs = [r[3] for r in rows]
        ax.plot(sizes_kb, psnrs, "o-", color=colors[codec_name], label=codec_name)
    ax.axhline(39.5, color="#777777", linestyle="--", linewidth=1)
    ax.set_xlabel("розмір, КБ")
    ax.set_ylabel("PSNR, дБ")
    ax.set_xscale("log")
    ax.grid(alpha=0.3)
    ax.legend()
    fig.tight_layout()
    fig.savefig(name, dpi=130)
    plt.close(fig)
    print("   збережено:", name)


# ════════════════════════════════════════════════════════════════════
#  ВАША ЧАСТИНА. Три місця.
# ════════════════════════════════════════════════════════════════════

def encode(out_name, codec="libx264", crf=23, preset="medium", extra=None):
    """Закодувати еталонний кліп. Повернути (шлях, секунди, розмір_у_байтах).

    Збирається звичайний рядок запуску ffmpeg:

        ffmpeg -y -hide_banner -loglevel error -i <SRC>
            -c:v <codec> -crf <crf> -preset <preset>
            -pix_fmt yuv420p -color_range tv <вихідний файл>

    У вигляді списку для subprocess це:

        path = os.path.join(WORK, out_name)
        cmd = ["ffmpeg", "-y", "-hide_banner", "-loglevel", "error", "-i", SRC,
            "-c:v", codec, "-preset", preset,
            "-pix_fmt", "yuv420p", "-color_range", "tv"]
        if crf is not None:                  # crf=None — коли бітрейт задають через extra
            cmd += ["-crf", str(crf)]
        cmd += (extra or [])
        cmd += [path]

    Чому crf доводиться вміти вимикати: якщо передати і -crf, і -b:v,
    ffmpeg не лається, а мовчки видає третій результат, не схожий ні на
    те, ні на те. Разом ці два прапорці ставити не можна.

    Час міряйте навколо самого запуску:
        t = time.perf_counter();  r = run(cmd);  dt = time.perf_counter() - t

    Якщо r.returncode не нуль — підніміть RuntimeError із r.stderr,
    інакше шукати помилку доведеться наосліп.

    Розмір файлу: os.path.getsize(path)
    """
    path = os.path.join(WORK, out_name)
    cmd = ["ffmpeg", "-y", "-hide_banner", "-loglevel", "error", "-i", SRC, "-c:v", codec, "-preset", preset, "-pix_fmt", "yuv420p", "-color_range", "tv"]
    if crf is not None:
        cmd += ["-crf", str(crf)]
    cmd += (extra or [])
    cmd += [path]

    t = time.perf_counter()
    r = run(cmd)
    dt = time.perf_counter() - t
    if r.returncode:
        raise RuntimeError("ffmpeg не зміг закодувати відео:\n" + r.stderr[-500:])
    return path, dt, os.path.getsize(path)


def measure(path, src=SRC):
    """Порівняти файл із еталоном. Повернути (psnr, ssim).

    Для кожної з двох метрик запускається ffmpeg з двома входами:

        cmd = ["ffmpeg", "-hide_banner", "-i", path, "-i", src,
            "-lavfi", CMP.format(fps=FPS) + metric, "-f", "null", "-"]
        r = run(cmd)

    ffmpeg пише підсумок у stderr, НЕ у stdout. Виглядає він так:

        [Parsed_psnr_0 @ ...] PSNR y:39.74 u:45.1 v:44.8 average:39.74 min:... max:...
        [Parsed_ssim_0 @ ...] SSIM Y:0.96 U:0.98 V:0.98 All:0.9682 (14.98)

    Звідси беремо число після average: для PSNR і після All: для SSIM:

        m = re.search(r"average:([\\d.]+)", r.stderr)      # для psnr
        m = re.search(r"All:([\\d.]+)", r.stderr)          # для ssim
        значення = float(m.group(1))

    Обидві метрики рахуйте в циклі for metric in ("psnr", "ssim").
    """
    values = {}
    for metric in ("psnr", "ssim"):
        cmd = ["ffmpeg", "-hide_banner", "-i", path, "-i", src, "-lavfi", CMP.format(fps=FPS) + metric, "-f", "null", "-"]
        r = run(cmd)
        if r.returncode:
            raise RuntimeError(f"ffmpeg не зміг обчислити {metric.upper()}:\n" + r.stderr[-500:])
        pattern = r"average:([\d.]+)" if metric == "psnr" else r"All:([\d.]+)"
        m = re.search(pattern, r.stderr)
        if not m:
            raise RuntimeError(f"ffmpeg не повернув значення {metric.upper()}:\n" + r.stderr[-500:])
        values[metric] = float(m.group(1))
    return values["psnr"], values["ssim"]


def ladder(crfs=(18, 23, 28, 33, 40), codec="libx264", preset="medium"):
    """Прогнати драбинку якостей. Повернути список рядків
    [crf, розмір, psnr, ssim, секунди].

    Каркас:
        rows = []
        for crf in crfs:
            path, dt, size = encode(f"q{crf}.mp4", codec, crf, preset)
            psnr, ssim = measure(path)
            rows.append([crf, size, psnr, ssim, dt])
            print(...)        # друкуйте одразу, щоб бачити поступ
        return rows

    Бітрейт рахується з розміру: size * 8 / тривалість / 1000 кбіт/с,
    тривалість тут NF / FPS = 5 секунд.
    """
    rows = []
    for crf in crfs:
        path, dt, size = encode(f"q{crf}.mp4", codec, crf, preset)
        psnr, ssim = measure(path)
        rows.append([crf, size, psnr, ssim, dt])
        duration = NF / FPS
        bitrate = size * 8 / duration / 1000
        print(f"   CRF {crf}: {size / 1024:.0f} КБ, " f"{bitrate:.0f} кбіт/с, PSNR {psnr:.2f} дБ, " f"SSIM {ssim:.4f}, час {dt:.2f} с")
    return rows


# ════════════════════════════════════════════════════════════════════
#  ТЕСТИ
# ════════════════════════════════════════════════════════════════════

def check(label, ok, detail=""):
    print(f"   [{'OK ' if ok else 'НІ '}] {label}" + ("" if ok or not detail else f"   — {detail}"))
    if not ok:
        print("\n   Зупиняюся. Виправте це місце й запустіть знову.")
        raise SystemExit(1)


def self_test():
    print("\n0. Чи є ffmpeg")
    r = run(["ffmpeg", "-version"])
    check("ffmpeg знайдено", r.returncode == 0, "встановіть ffmpeg і додайте в PATH")
    check("ffprobe знайдено", run(["ffprobe", "-version"]).returncode == 0)

    print("\n1. Еталонний кліп")
    make_source()
    d = probe(SRC)
    check("роздільність 1280×720", (d.get("width"), d.get("height")) == ("1280", "720"), f"{d.get('width')}×{d.get('height')}")
    check("кодек без втрат ffv1", d.get("codec_name") == "ffv1", d.get("codec_name"))

    print("\n2. Кодування")
    path, dt, size = encode("t23.mp4", crf=23)
    check("файл створено", os.path.exists(path))
    check("розмір близько 556 КБ", abs(size / 1024 - 556) < 120, f"{size/1024:.0f} КБ")
    check("час вимірюється", dt > 0.2, f"{dt:.1f} с")
    check("усередині H.264", probe(path).get("codec_name") == "h264")

    print("\n3. Вимірювання")
    ps, ss = measure(path)
    check("PSNR близько 39,7 дБ", abs(ps - 39.74) < 1.0, f"{ps:.2f}")
    check("SSIM близько 0,968", abs(ss - 0.9682) < 0.01, f"{ss:.4f}")
    _, _, s40 = encode("t40.mp4", crf=40)
    p40, _ = measure(os.path.join(WORK, "t40.mp4"))
    check("гірша якість дає менший файл", s40 < size, f"{s40/1024:.0f} проти {size/1024:.0f} КБ")
    check("і менший PSNR", p40 < ps, f"{p40:.2f} проти {ps:.2f}")

    print("\n4. Драбинка")
    rows = ladder((23, 33))
    check("повернено два рядки", len(rows) == 2, str(len(rows)))
    check("з п'яти чисел кожен", all(len(r) == 5 for r in rows))

    print("\n   Усі тести пройдено. Далі — завдання 3, 4 і 5.\n")


def run_all():
    print("Драбинка CRF")
    print(f"   {'crf':>5}{'розмір':>11}{'бітрейт':>12}{'PSNR':>8}{'SSIM':>9}{'час':>8}")
    rows = []
    for crf in (18, 23, 28, 33, 40):
        path, dt, size = encode(f"q{crf}.mp4", crf=crf)
        ps, ss = measure(path)
        rows.append([crf, size, ps, ss, dt])
        print(f"   {crf:>5}{size/1024:>9.0f} КБ{size*8/5/1000:>10.0f} кб/с" f"{ps:>8.2f}{ss:>9.4f}{dt:>7.1f}с")
    plot_rd(rows)

    print("\nТа сама драбинка, виміряна НЕПРАВИЛЬНО (без вирівнювання міток часу)")
    print(f"   {'crf':>5}{'правильно':>12}{'неправильно':>14}")
    for crf, size, ps, ss, dt in rows:
        r = run(["ffmpeg", "-hide_banner", "-i", os.path.join(WORK, f"q{crf}.mp4"), "-i", SRC, "-lavfi", "psnr", "-f", "null", "-"])
        m = re.search(r"average:([\d.]+)", r.stderr)
        print(f"   {crf:>5}{ps:>11.2f}{float(m.group(1)):>13.2f}")
    print("   Праворуч майже стала величина: таке вимірювання не бачить різниці")
    print("   між найкращою і найгіршою якістю. Чому — у методичці, розділ 2.4.")

    print("\nПорівняння H.264, H.265 і AV1 при однаковій якості")
    print(f"   {'кодек':<7}{'crf':>5}{'розмір':>11}{'PSNR':>8}{'час':>8}")
    jobs = [
        ("H.264", "libx264", [18, 23, 28]),
        ("H.265", "libx265", [20, 25, 30]),
        ("AV1", "libsvtav1", [25, 35, 45]),
    ]
    job_rows = []
    for name, codec, qs in jobs:
        rows = []
        for q in qs:
            preset = "8" if codec == "libsvtav1" else "medium"
            path, dt, size = encode(
                f"{codec}_{q}.mp4", codec=codec, crf=q, preset=preset
            )
            psnr, ssim = measure(path)
            rows.append([q, dt, size, psnr, ssim])
            print(f"   {name:<7}{q:>5}{size/1024:>9.0f} КБ" f"{psnr:>8.2f}{dt:>7.1f}с")
        job_rows.append((name, rows))
    plot_codecs(job_rows)


if __name__ == "__main__":
    try:
        self_test()
    except NotImplementedError as e:
        print(f"\n   Місце {e} ще не зроблено. Знайдіть його у файлі за словом TODO,")
        print("   заповніть за підказкою поруч — і запустіть файл знову.\n")
        raise SystemExit(1)
    run_all()
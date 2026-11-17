from pathlib import Path

import matplotlib

matplotlib.use("Agg")
import matplotlib.pyplot as plt
import numpy as np
from scipy.io import wavfile
from scipy.signal import spectrogram


SR = 44100
OUT = Path(__file__).resolve().parent


def music(dur=3.0, sr=SR):
    t = np.arange(int(dur * sr)) / sr
    x = np.zeros_like(t)
    for f0 in (220.0, 277.18, 329.63):
        for k, amplitude in enumerate((1.0, 0.5, 0.3, 0.18, 0.1, 0.06), start=1):
            x += amplitude * np.sin(2 * np.pi * f0 * k * t + k)
    x *= np.minimum(1, t * 8) * np.exp(-0.35 * t)
    return 0.85 * x / np.abs(x).max()


def save_wav(name, x, sr=SR):
    x = np.asarray(x, dtype=float)
    peak = np.max(np.abs(x))
    if peak > 1:
        x = x / peak
    wavfile.write(OUT / name, sr, (np.clip(x, -1, 1) * 32767).astype(np.int16))
    print(f"Збережено {name}: {len(x) / sr:.2f} с, {sr} Гц")


def get_window_spectra(n=1024, bin_position=100.0):
    t = np.arange(n) / SR
    frequency = bin_position * SR / n
    signal = np.sin(2 * np.pi * frequency * t)
    windows = {
        "Прямокутне": np.ones(n),
        "Ганна": np.hanning(n),
        "Блекмана": np.blackman(n),
    }
    spectra = {}
    leakage = {}
    for name, window in windows.items():
        spectrum = np.abs(np.fft.rfft(signal * window))
        db = 20 * np.log10(spectrum / spectrum.max() + 1e-15)
        far_bins = np.concatenate((db[:95], db[106:]))
        spectra[name] = db
        leakage[name] = float(far_bins.max())
    return frequency, spectra, leakage


def time_frequency_signal(sr=SR):
    duration = 1.4
    t = np.arange(int(duration * sr)) / sr
    x = np.zeros_like(t)
    tones = t < 0.6
    x[tones] = 0.5 * np.sin(2 * np.pi * 440 * t[tones])
    x[tones] += 0.5 * np.sin(2 * np.pi * 480 * t[tones])
    for center in (0.80, 0.92, 1.04, 1.16):
        start = int(center * sr)
        x[start:start + 40] += np.hanning(40)
    return x


def equalize(x, bands, sr=SR):
    """Apply non-overlapping (low Hz, high Hz, gain dB) bands in the FFT domain."""
    x = np.asarray(x, dtype=float)
    nyquist = sr / 2
    gains = np.ones(np.fft.rfft(x).size)
    frequencies = np.fft.rfftfreq(len(x), 1 / sr)
    previous_high = 0.0
    for low, high, db in sorted(bands):
        if not (0 <= low < high <= nyquist):
            raise ValueError(f"Invalid EQ band: ({low}, {high}) for Nyquist {nyquist}")
        if low < previous_high:
            raise ValueError("EQ bands must not overlap")
        if not np.isfinite(db):
            raise ValueError("EQ gain must be finite")
        gains[(frequencies >= low) & (frequencies < high)] = 10 ** (db / 20)
        previous_high = high
    return np.fft.irfft(np.fft.rfft(x) * gains, n=len(x))


def band_energy_db(x, low, high, sr=SR):
    spectrum = np.abs(np.fft.rfft(x))
    frequencies = np.fft.rfftfreq(len(x), 1 / sr)
    energy = np.sum(spectrum[(frequencies >= low) & (frequencies < high)] ** 2)
    return float(10 * np.log10(energy + 1e-20))


def save_spectrum_and_spectrogram(x):
    n = 8192
    spectrum = np.abs(np.fft.rfft(x[:n] * np.hanning(n)))
    db = 20 * np.log10(spectrum / spectrum.max() + 1e-12)
    frequencies = np.fft.rfftfreq(n, 1 / SR)
    mask = frequencies < 2500
    for expected in (220.0, 277.18, 329.63):
        nearby = np.abs(frequencies - expected) <= 8
        measured = frequencies[nearby][np.argmax(spectrum[nearby])]
        if abs(measured - expected) > 5:
            raise AssertionError(
                f"Expected a spectral peak near {expected:.2f} Hz, got {measured:.2f} Hz"
            )

    fig, ax = plt.subplots(figsize=(9, 4))
    ax.plot(frequencies[mask], db[mask], lw=0.9)
    ax.set(title="Спектр музичного сигналу", xlabel="Частота, Гц", ylabel="Рівень, дБ")
    ax.set_ylim(-100, 5)
    ax.grid(alpha=0.3)
    fig.tight_layout()
    fig.savefig(OUT / "spectrum.png", dpi=150)
    plt.close(fig)

    frequencies, times, power = spectrogram(
        x, SR, nperseg=2048, noverlap=1792
    )
    fig, ax = plt.subplots(figsize=(10, 4.5))
    image = ax.pcolormesh(
        times, frequencies, 10 * np.log10(power + 1e-12),
        shading="auto", cmap="magma",
    )
    ax.set(
        title="Спектрограма музичного сигналу",
        xlabel="Час, с",
        ylabel="Частота, Гц",
        ylim=(0, 2500),
    )
    fig.colorbar(image, ax=ax, label="Потужність, дБ")
    fig.tight_layout()
    fig.savefig(OUT / "spectrogram.png", dpi=150)
    plt.close(fig)


def save_window_comparison():
    cases = (
        ("Рівно на біні", 100.0),
        ("Між бінами (0,5 біна)", 100.5),
    )
    fig, axes = plt.subplots(2, 3, figsize=(13, 7), sharex=True, sharey=True)
    results = {}
    for row, (case_name, bin_position) in enumerate(cases):
        frequency, spectra, leakage = get_window_spectra(bin_position=bin_position)
        results[case_name] = leakage
        for col, (name, db) in enumerate(spectra.items()):
            axes[row, col].plot(np.arange(len(db)), db, lw=0.9)
            axes[row, col].set_title(f"{case_name}\n{name}")
            axes[row, col].set_xlim(70, 130)
            axes[row, col].set_ylim(-100, 3)
            axes[row, col].grid(alpha=0.25)
            axes[row, col].set_xlabel("Номер біна")
            if col == 0:
                axes[row, col].set_ylabel("Рівень, дБ")
        print(f"\n{case_name}, f = {frequency:.2f} Гц")
        for name, value in leakage.items():
            print(f"  {name}: сторонній пік {value:.2f} дБ")
    fig.suptitle("Вплив вікна на спектральне розтікання", y=1.01)
    fig.tight_layout()
    fig.savefig(OUT / "window_comparison.png", dpi=150, bbox_inches="tight")
    plt.close(fig)
    return results


def save_time_frequency_comparison():
    x = time_frequency_signal()
    for n in (128, 4096):
        frequencies, times, power = spectrogram(
            x, SR, nperseg=n, noverlap=int(n * 0.9)
        )
        fig, ax = plt.subplots(figsize=(10, 4.5))
        image = ax.pcolormesh(
            times,
            frequencies,
            10 * np.log10(power + 1e-12),
            shading="auto",
            cmap="magma",
            vmin=-100,
            vmax=-20,
        )
        ax.set(
            title=f"Спектрограма: вікно {n} відліків",
            xlabel="Час, с",
            ylabel="Частота, Гц",
            ylim=(0, 1200),
        )
        fig.colorbar(image, ax=ax, label="Потужність, дБ")
        fig.tight_layout()
        fig.savefig(OUT / f"spectrogram_{n}.png", dpi=150)
        plt.close(fig)


def save_equalizer_results(x):
    bands_to_measure = (
        (0, 250),
        (250, 1000),
        (1000, 4000),
        (4000, SR // 2),
    )
    test = equalize(x, [(0, 250, 12)])
    print("\nПеревірка EQ (+12 дБ у смузі 0–250 Гц)")
    for low, high in bands_to_measure:
        before = band_energy_db(x, low, high)
        after = band_energy_db(test, low, high)
        print(f"  {low:>5}–{high:<5} Гц: {before:.2f} -> {after:.2f} дБ ({after - before:+.2f} дБ)")
        expected = 12.0 if (low, high) == (0, 250) else 0.0
        if not np.isclose(after - before, expected, atol=0.01):
            raise AssertionError(f"EQ energy check failed for {low}–{high} Hz")

    spectrum_before = np.abs(np.fft.rfft(x))
    spectrum_after = np.abs(np.fft.rfft(test))
    frequencies = np.fft.rfftfreq(len(x), 1 / SR)
    reference = spectrum_before.max()
    fig, ax = plt.subplots(figsize=(10, 4.5))
    mask = frequencies < 5000
    ax.plot(
        frequencies[mask],
        20 * np.log10(spectrum_before[mask] / reference + 1e-12),
        label="До еквалайзера",
        lw=0.8,
    )
    ax.plot(
        frequencies[mask],
        20 * np.log10(spectrum_after[mask] / reference + 1e-12),
        label="Після: +12 дБ у смузі 0–250 Гц",
        lw=0.8,
    )
    ax.set(title="Спектр до та після еквалайзера", xlabel="Частота, Гц", ylabel="Рівень, дБ")
    ax.grid(alpha=0.3)
    ax.legend()
    fig.tight_layout()
    fig.savefig(OUT / "equalizer.png", dpi=150)
    plt.close(fig)

    presets = {
        "preset_bass.wav": [(0, 250, 6)],
        "preset_voice.wav": [(0, 300, -2), (300, 3400, 4), (3400, 11025, -2)],
        "preset_telephone.wav": [(0, 300, -60), (3000, SR // 2, -60)],
    }
    for filename, bands in presets.items():
        save_wav(filename, equalize(x, bands))
    return bands_to_measure, [
        (band_energy_db(x, low, high), band_energy_db(test, low, high))
        for low, high in bands_to_measure
    ]


def main():
    source = music()
    save_wav("source.wav", source)
    save_spectrum_and_spectrogram(source)
    leakage = save_window_comparison()
    save_time_frequency_comparison()
    bands, energies = save_equalizer_results(source)

    on_bin_rect = leakage["Рівно на біні"]["Прямокутне"]
    off_bin = leakage["Між бінами (0,5 біна)"]
    if on_bin_rect > -250:
        raise AssertionError(f"On-bin rectangular leakage too high: {on_bin_rect:.2f} dB")
    if not -24 <= off_bin["Прямокутне"] <= -18:
        raise AssertionError("Off-bin rectangular leakage is outside the expected range")
    if not -57 <= off_bin["Ганна"] <= -48:
        raise AssertionError("Off-bin Hann leakage is outside the expected range")
    if not -66 <= off_bin["Блекмана"] <= -58:
        raise AssertionError("Off-bin Blackman leakage is outside the expected range")

    print("\nТаблиця енергій для звіту")
    for (low, high), (before, after) in zip(bands, energies):
        print(f"  {low:>5}–{high:<5} Гц | {before:.2f} | {after:.2f} | {after - before:+.2f}")
    print("\nУсі автоматичні перевірки пройдено.")


if __name__ == "__main__":
    main()

<div align="center">

<img src="https://img.shields.io/badge/Engine-Unity-black?logo=unity" alt="Unity"/> <img src="https://img.shields.io/badge/Platform-PC%20(Windows)-blue" alt="Platform"/> <img src="https://img.shields.io/badge/Genre-2D%20Action--Adventure-orange" alt="Genre"/> <img src="https://img.shields.io/badge/Status-In%20Development-yellow" alt="Status"/> <img src="https://img.shields.io/badge/Rating-13%2B-red" alt="Rating"/>

# SECTOR.09

### *Kota yang berhenti mempercayai sistemnya sendiri.*

**2D Action-Adventure Side-Scrolling — Cyberpunk Undercity**

Chevalier Laboratory · Game Development · Universitas Telkom Bandung · 2026

</div>

---

## 📖 Tentang Game

**SECTOR.09** adalah game **2D action-adventure side-scrolling** bergaya kota bawah tanah cyberpunk. Pemain berperan sebagai seorang **Scavenger** — penjelajah independen yang menyusuri reruntuhan sebuah distrik kota yang terputus dari jaringan pusat setelah kegagalan sistem AI kota. Insiden ini membuat sebagian robot lepas kendali (*rogue AI*) dan memperebutkan sisa-sisa teknologi bersama kelompok manusia yang tertinggal.

Dalam perjalanannya, pemain melakukan eksplorasi sambil bertarung melawan drone-drone penjaga untuk bertahan hidup dan mengungkap sisa teknologi yang ditinggalkan.

| | |
|---|---|
| **Genre** | 2D Action-Adventure, Side-Scrolling |
| **Platform** | PC (Windows) |
| **Engine** | Unity |
| **Target Audience** | 13+ tahun |
| **Tim** | Tim 1 — Scraplight |

---

## 🎮 Gameplay Loop

Core loop SECTOR.09 berputar pada empat fase yang berulang di setiap area:

```
Eksplorasi → Encounter → Combat → Progress
```

1. **Eksplorasi** — menyusuri lingkungan side-scrolling dan memanfaatkan platforming (*jump*, *double jump*) untuk mencapai area baru.
2. **Encounter** — bertemu drone yang berpatroli menjaga area tertentu di sepanjang jalur.
3. **Combat** — menyerang musuh dengan mekanik *attack*, dengan opsi *dash* untuk menghindari serangan balik.
4. **Progress** — menyelesaikan objektif area untuk membuka jalur menuju bagian sector berikutnya.

### Moveset Pemain (Scavenger)

| Aksi | Deskripsi |
|---|---|
| **Run** | Pergerakan horizontal dasar karakter. |
| **Jump & Double Jump** | Mencapai platform bertingkat dan menghindari rintangan. |
| **Dash** | Pergerakan cepat jarak pendek, sekaligus mekanik evasi menghindari serangan musuh. |
| **Attack** | Serangan jarak dekat untuk melumpuhkan musuh. |

### Combat Feedback

| Sistem | Fungsi |
|---|---|
| **Health (HP)** | Berkurang saat pemain terkena serangan musuh — risiko nyata di tiap encounter. |
| **I-Frame** | Jeda kekebalan singkat setelah terkena damage, mencegah damage beruntun. |
| **Knockback** | Pemain (dan musuh) terpental saat terkena serangan, memberi umpan balik dampak combat yang jelas. |

### Musuh — Drone AI

Drone berpatroli di sepanjang level menggunakan sistem AI berbasis *state machine* sederhana:

`Patrol` (hover bobbing di area jaga) → `Chase` (mengejar saat pemain terdeteksi) → `Attack` (menyerang dalam jarak serang)

Musuh tambahan (raiders bersenjata / varian drone lain) berpotensi ditambahkan sebagai fitur bonus inovasi.

---

## 🖼️ Art Style

SECTOR.09 menggunakan gaya visual **2D Pixel Art** dengan tema kontras: lingkungan kota bawah tanah yang gelap dan berkarat dipadukan dengan pendaran lampu neon terang pada elemen teknologi (layar digital, papan tanda, robot/AI).

**Palet Warna**

| Warna | Kode | Fungsi |
|---|---|---|
| ![#1E2537](https://placehold.co/15x15/1E2537/1E2537.png) | `#1E2537` | Dark Sky / Shadow — latar belakang gelap, langit malam, gang tanpa cahaya |
| ![#435D69](https://placehold.co/15x15/435D69/435D69.png) | `#435D69` | Base Environment — struktur bangunan, dinding kota, pipa besi |
| ![#4F98C1](https://placehold.co/15x15/4F98C1/4F98C1.png) | `#4F98C1` | Cold Tech Light — panel kontrol, monitor, layar teknologi lawas |
| ![#34A7A4](https://placehold.co/15x15/34A7A4/34A7A4.png) | `#34A7A4` | Neon Cyan — papan tanda toko, teks digital, terminal aktif |
| ![#F26257](https://placehold.co/15x15/F26257/F26257.png) | `#F26257` | Neon Coral / Pink — pencahayaan distrik, lampu reklame & peringatan |
| ![#EC6C23](https://placehold.co/15x15/EC6C23/EC6C23.png) | `#EC6C23` | Warm Amber / Orange — lampu darurat, lentera, cahaya buatan manusia |

---

## 🗺️ Level Design

- **Grid & Tileset** — dibangun dengan sistem **Tilemap** Unity; cell size & sorting order konsisten agar layering environment (background, struktur, foreground) rapi.
- **Perspektif** — side-scrolling 2D, jalur linear dengan variasi vertikal untuk memanfaatkan mekanik *jump* & *double jump*.
- **Penempatan Musuh** — drone diletakkan strategis di area yang menuntut kombinasi platforming dan combat.
- **Storytelling Lingkungan** — papan tanda neon, reruntuhan kendaraan, dan sisa infrastruktur memperkuat nuansa undercity tanpa dialog tambahan.

---

## ✅ Kriteria Keberhasilan (MVP)

- [x] **Main Menu** — tombol interaktif (Start Game, Setting, Exit) sebelum masuk gameplay inti
- [x] **Gameplay Loop** — eksplorasi → encounter → combat → progres berjalan tanpa game-breaking bug
- [x] **UI Design** — HUD (health, indikator status) jelas, intuitif, mudah dibaca
- [x] **Setting & Level Design** — lingkungan undercity cyberpunk konsisten di seluruh level
- [x] **Objective** — tujuan tiap level/area tersampaikan jelas sejak awal
- [x] **Art Style Konsisten** — seluruh aset mengikuti palet warna & gaya pixel art yang ditentukan
- [x] **Performa** — FPS stabil, tanpa crash pada perangkat PC target

---

## 🛠️ Tech Stack

- **Engine:** Unity
- **Bahasa:** C#
- **Tools:** Unity Tilemap System, Unity Input System, 2D Pixel Art pipeline

---

## 🚀 Cara Menjalankan

### Build (langsung main)
1. Unduh build terbaru dari folder/release repo ini.
2. Ekstrak file `.rar` / `.zip`.
3. Jalankan `SECTOR 09.exe`.

### Dari Source (Unity Editor)
1. Clone repo ini:
   ```bash
   git clone https://github.com/Slugazoid/2D-Game-Chevalier-Lab.git
   ```
2. Buka project menggunakan **Unity Hub** dengan versi Unity yang sesuai.
3. Buka scene utama, lalu tekan **Play** untuk menjalankan di dalam Editor.

---

## 👥 Tim — Tim 1 · Scraplight

Tim lintas peran yang bekerja bersama menutupi Game Design, Programming, Art & UI/UX, hingga Audio dalam satu siklus produksi.

| Nama | NIM |
|---|---|
| Albert Sebastian Tobing | 
| Muhammad Daffa Arifin |
| Ghaisani Zhafarina |

**Chevalier Laboratory · Game Development · Universitas Telkom Bandung, 2026**

---

## 📄 Lisensi

Proyek ini dibuat untuk keperluan akademik (Final Project Innovation Challenge) di Universitas Telkom Bandung. Seluruh hak cipta aset dan kode berada pada Tim 1 — Scraplight kecuali dinyatakan lain.

---

<div align="center">

*Ship it. Play it. Learn from it. Improve it.*

</div>

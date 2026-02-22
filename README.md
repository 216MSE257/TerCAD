# TerCAD
Drawing with terminal commands is possible with a simple CAD kernel

TURKISH:

# 📐 TerCAD Engine 

WPF ve C# kullanılarak geliştirilmiş, mühendislik hassasiyetine sahip hafif bir CAD yazılımı.

## ✨ Özellikler
* **Temel Çizim:** Çizgi, Dikdörtgen, Daire, Çokgen.
* **Mühendislik Araçları:** Akıllı Snap (Uç ve Orta nokta), Ortho Modu (F8).
* **Düzenleme:** Geri Al/İleri Al, Döndürme, Ölçekleme, Renk/Kalınlık değiştirme.
* **Dışa Aktarma:** Çizimleri **DXF (AutoCAD)** formatında kaydedebilme.
* **Görsel:** Dinamik Izgara (Grid) sistemi ve koordinat takibi.

## 🚀 Kurulum ve Çalıştırma
1. Bu depoyu klonlayın: `git clone https://github.com/kullaniciadi/MiniCadProject.git`
2. Klasöre gidin: `cd MiniCadProject`
3. Çalıştırın: `dotnet run`

## ⌨️ Komut Örnekleri
- `L 0,0 100,100` : Çizgi çizer.
- `REC 10,10 @50,50` : 50x50 kare çizer.
- `EXP` : DXF çıktısı alır.

- ENGLISH:

- # 📐 TerCAD Engine 

A lightweight CAD software with engineering precision, developed using WPF and C#.

## ✨ Features
* **Basic Drawing:** Line, Rectangle, Circle, Polygon.
* **Engineering Tools:** Smart Snap (End and Midpoint), Ortho Mode (F8).
* **Editing:** Undo/Redo, Rotate, Scaling, Color/Thickness change.
* **Export:** Ability to save drawings in **DXF (AutoCAD)** format.
* **Image:** Dynamic Grid system and coordinate tracking.

## 🚀 Installation and Operation
1. Clone this repository: `git clone https://github.com/username/MiniCadProject.git`
2. Go to folder: `cd MiniCadProject`
3. Run: `dotnet run`

## ⌨️ Command Examples
- `L 0,0 100,100` : Draws a line.
- `REC 10,10 @50,50` : Draws a 50x50 square.
- `EXP` : Gets DXF output.

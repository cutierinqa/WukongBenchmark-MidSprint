using System.IO;
using Tesseract;

namespace WukongBenchmark.Services;

public class OcrService
{
    public string ReadText(string imagePath)
    {
        string tessdataPath = Path.Combine(
            AppContext.BaseDirectory,
            "tessdata");

        using var engine = new TesseractEngine(
            tessdataPath,
            "eng",
            EngineMode.Default);

        using var image = Pix.LoadFromFile(imagePath);
        using var page = engine.Process(image);

        return page.GetText();
    }
    public string ReadRegion(
    string imagePath,
    int x,
    int y,
    int width,
    int height)
    {
        string tessdataPath = Path.Combine(
            AppContext.BaseDirectory,
            "tessdata");

        using var original = System.Drawing.Bitmap.FromFile(imagePath);

        var rectangle = new System.Drawing.Rectangle(
            x,
            y,
            width,
            height);

        using var cropped = new System.Drawing.Bitmap(
            width,
            height);

        using (var graphics =
            System.Drawing.Graphics.FromImage(cropped))
        {
            graphics.DrawImage(
                original,
                new System.Drawing.Rectangle(0, 0, width, height),
                rectangle,
                System.Drawing.GraphicsUnit.Pixel);
        }

        string tempPath = Path.Combine(
            Path.GetTempPath(),
            "wukong_ocr_region.png");

        cropped.Save(
            tempPath,
            System.Drawing.Imaging.ImageFormat.Png);

        using var engine = new TesseractEngine(
            tessdataPath,
            "eng",
            EngineMode.Default);

        using var image = Pix.LoadFromFile(tempPath);
        using var page = engine.Process(image);

        return page.GetText().Trim();
    }
    public string ReadNumbers(
    string imagePath,
    int x,
    int y,
    int width,
    int height)
    {
        string tessdataPath = Path.Combine(
            AppContext.BaseDirectory,
            "tessdata");

        using var original =
            System.Drawing.Bitmap.FromFile(imagePath);

        var rectangle = new System.Drawing.Rectangle(
            x,
            y,
            width,
            height);

        using var cropped =
            new System.Drawing.Bitmap(width, height);

        using (var graphics =
            System.Drawing.Graphics.FromImage(cropped))
        {
            graphics.DrawImage(
                original,
                new System.Drawing.Rectangle(
                    0,
                    0,
                    width,
                    height),
                rectangle,
                System.Drawing.GraphicsUnit.Pixel);
        }

        int scale = 3;

        using var enlarged =
            new System.Drawing.Bitmap(
                width * scale,
                height * scale);

        using (var graphics =
            System.Drawing.Graphics.FromImage(enlarged))
        {
            graphics.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            graphics.DrawImage(
                cropped,
                new System.Drawing.Rectangle(
                    0,
                    0,
                    enlarged.Width,
                    enlarged.Height));
        }

        string debugPath = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop),
            "wukong_ocr_numbers_debug.png");

        enlarged.Save(
            debugPath,
            System.Drawing.Imaging.ImageFormat.Png);

        string tempPath = Path.Combine(
            Path.GetTempPath(),
            "wukong_ocr_numbers.png");

        enlarged.Save(
            tempPath,
            System.Drawing.Imaging.ImageFormat.Png);

        using var engine = new TesseractEngine(
            tessdataPath,
            "eng",
            EngineMode.Default);

        engine.SetVariable(
            "tessedit_char_whitelist",
            "0123456789");

        using var image =
            Pix.LoadFromFile(tempPath);

        using var page =
            engine.Process(image);

        return page.GetText().Trim();
    }
    public string ReadFirstNumber(
    string imagePath,
    int x,
    int y,
    int width,
    int height)
    {
        string text = ReadNumbers(
            imagePath,
            x,
            y,
            width,
            height);

        var match =
            System.Text.RegularExpressions.Regex.Match(
                text,
                @"\d+(?:[.,]\d+)?");

        return match.Success
            ? match.Value
            : "Не найдено";
    }
    public string ReadVramNumber(
    string imagePath,
    int x,
    int y,
    int width,
    int height)
    {
        string tessdataPath = Path.Combine(
            AppContext.BaseDirectory,
            "tessdata");

        using var original =
            System.Drawing.Bitmap.FromFile(imagePath);

        var rectangle = new System.Drawing.Rectangle(
            x,
            y,
            width,
            height);

        using var cropped =
            new System.Drawing.Bitmap(width, height);

        using (var graphics =
            System.Drawing.Graphics.FromImage(cropped))
        {
            graphics.DrawImage(
                original,
                new System.Drawing.Rectangle(
                    0,
                    0,
                    width,
                    height),
                rectangle,
                System.Drawing.GraphicsUnit.Pixel);
        }

        int scale = 3;

        using var enlarged =
            new System.Drawing.Bitmap(
                width * scale,
                height * scale);

        using (var graphics =
            System.Drawing.Graphics.FromImage(enlarged))
        {
            graphics.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            graphics.DrawImage(
                cropped,
                new System.Drawing.Rectangle(
                    0,
                    0,
                    enlarged.Width,
                    enlarged.Height));
        }

        string tempPath = Path.Combine(
            Path.GetTempPath(),
            "wukong_ocr_vram.png");

        enlarged.Save(
            tempPath,
            System.Drawing.Imaging.ImageFormat.Png);

        using var engine = new TesseractEngine(
            tessdataPath,
            "eng",
            EngineMode.Default);

        engine.SetVariable(
            "tessedit_char_whitelist",
            "0123456789,.");

        using var image =
            Pix.LoadFromFile(tempPath);

        using var page =
            engine.Process(image);

        string text = page.GetText().Trim();

        text = text.Replace(".", ",");

        var match =
            System.Text.RegularExpressions.Regex.Match(
                text,
                @"\d+,\d+");

        return match.Success
            ? match.Value
            : text;
    }
    public string ReadWordsWithPositions(string imagePath)
    {
        string tessdataPath = Path.Combine(
            AppContext.BaseDirectory,
            "tessdata");

        using var engine = new TesseractEngine(
            tessdataPath,
            "eng",
            EngineMode.Default);

        using var image = Pix.LoadFromFile(imagePath);
        using var page = engine.Process(image);

        using var iterator = page.GetIterator();

        iterator.Begin();

        var result = new System.Text.StringBuilder();

        do
        {
            string? word = iterator.GetText(
                PageIteratorLevel.Word);

            if (string.IsNullOrWhiteSpace(word))
                continue;

            if (iterator.TryGetBoundingBox(
                PageIteratorLevel.Word,
                out var rect))
            {
                result.AppendLine(
                    $"{word} → X:{rect.X1} Y:{rect.Y1} " +
                    $"W:{rect.Width} H:{rect.Height}");
            }

        } while (iterator.Next(
            PageIteratorLevel.Word));

        return result.ToString();
    }
}
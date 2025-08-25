using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace AdminPortalV8.Services
{
    public interface IImageQualityAnalyzer
    {
        bool GetAverageBrightness(string imagePath);
        bool IsBlurry(string imagePath);
    }
    public class ImageBrightnessChecker : IImageQualityAnalyzer
    {
        public bool GetAverageBrightness(string imagePath)
        {
            using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
            {
                double totalBrightness = 0;
                int pixelCount = 0;

                // Loop through each pixel row
                for (int y = 0; y < image.Height; y++)
                {
                    var pixelRow = image.Width;

                    // Loop through each pixel in the row
                    for (int x = 0; x < pixelRow; x++)
                    {
                        Rgba32 pixel = image[x, y];
                        // Calculate perceived brightness
                        double brightness = 0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;
                        totalBrightness += brightness;
                        pixelCount++;
                    }
                }

                // Calculate and return the average brightness
                double brightnessThreshold = 50;
                double brightnessScore = totalBrightness / pixelCount;
                Console.WriteLine(brightnessScore);
                // Return True if the brightness score blow the threshold, inicationg dark
                return brightnessScore < brightnessThreshold;
            }
        }

        public bool IsBlurry(string imagePath)
        {
            using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
            {
                int edgeCount = 0;

                // Apply edge detection (Sobel filter)
                image.Mutate(ctx => ctx.DetectEdges());

                // Get pixel data for analysis
                var pixels = image.CloneAs<Rgba32>();
                for (int y = 0; y < pixels.Height; y++)
                {
                    for (int x = 0; x < pixels.Width - 1; x++) // Compare adjacent pixels
                    {
                        var pixel = pixels[x, y];
                        var nextPixel = pixels[x + 1, y];

                        // Calculate brightness for both pixels
                        double brightness = 0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;
                        double nextBrightness = 0.299 * nextPixel.R + 0.587 * nextPixel.G + 0.114 * nextPixel.B;

                        // If the brightness difference is high, count as an edge
                        if (Math.Abs(brightness - nextBrightness) > 50)
                        {
                            edgeCount++;
                        }
                    }
                }
                int edgeThreshold = 30000;
                // Return true if the edge count is below the threshold, indicating blur
                return edgeCount < edgeThreshold;
            }
        
        }


    }
}
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using WebAspDBeaverStudy.Interfaces;

namespace WebAspDBeaverStudy.Services
{
    public class ImageWorker : IImageWorker
    {
        private readonly IWebHostEnvironment _environment; // Зберігає інформацію про середовище хостингу
        private const string dirName = "uploading"; // Назва папки для збереження зображень
        private int[] sizes = { 50, 150, 300, 600, 1200 }; // Розміри зображень для збереження

        public ImageWorker(IWebHostEnvironment environment) // Конструктор, який приймає середовище хостингу
        {
            _environment = environment;
        }

        public string Save(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Send a GET request to the image URL
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    // Check if the response status code indicates success (e.g., 200 OK)
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the image bytes from the response content
                        byte[] imageBytes = response.Content.ReadAsByteArrayAsync().Result;
                        return CompresImage(imageBytes);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to retrieve image. Status code: {response.StatusCode}");
                        return String.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Виникла помилка: {ex.Message}");
                return String.Empty;
            }
        }

        /// <summary>
        /// Стискаємо фото
        /// </summary>
        /// <param name="bytes">Набір байтів фото</param>
        /// <returns>Повертаємо назву збереженого 
        private string CompresImage(byte[] bytes)
        {
            string imageName = Guid.NewGuid().ToString() + ".webp"; // Генеруємо унікальне ім'я зображення

            var dirSave = Path.Combine(_environment.WebRootPath, dirName); // Шлях до папки для збереження
            if (!Directory.Exists(dirSave)) // Якщо папка не існує створюємо папку
            {
                Directory.CreateDirectory(dirSave);
            }

            foreach (int size in sizes) // Проходимо по кожному розміру
            {
                var path = Path.Combine(dirSave, $"{size}_{imageName}"); // Формуємо шлях для збереження зображення
                using (var image = Image.Load(bytes)) // Завантажуємо зображення з байтів
                {
                    image.Mutate(x => x.Resize(new ResizeOptions // Змінюємо розмір зображення
                    {
                        Size = new Size(size, size), // Задаємо новий розмір
                        Mode = ResizeMode.Max // Режим зміни розміру
                    }));
                    image.SaveAsWebp(path); // Зберігаємо зображення у форматі WebP
                    //image.Save(path, new WebpEncoder());
                }
            }

            return imageName; // Повертаємо ім'я збереженого зображення
        }

        public void Delete(string fileName) // Метод для видалення зображення
        {
            foreach (int size in sizes)
            {
                var fileSave = Path.Combine(_environment.WebRootPath, dirName, $"{size}_{fileName}");
                if (File.Exists(fileSave)) // Якщо файл існує
                    File.Delete(fileSave); // Видаляємо файл
            }
        }

        public string Save(IFormFile file) // Метод для збереження зображення з форми
        {
            try
            {
                using (var memoryStream = new MemoryStream()) // Створюємо пам'ятковий потік для копіювання файлу
                {
                    file.CopyTo(memoryStream); // Копіюємо вміст файлу в пам'ятковий потік
                    byte[] imageBytes = memoryStream.ToArray(); // Перетворюємо потік на масив байтів
                    return CompresImage(imageBytes); // Виклик методу стиснення зображення
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Виникла помилка: {ex.Message}");
                return String.Empty;
            }
        }
    }
}

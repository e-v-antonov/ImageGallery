using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using ImageGallery.AdditionalClass;
using ImageGallery.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ImageGallery.Controllers
{
    public class UpdateImageController : Controller
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        private static Bitmap image;
        private static UInt32[,] pixel;
        private static byte[] bitmapBytes;
        private static string imgBase64;
        private static string imagePath = string.Empty;
        private static string nameImage = string.Empty;
        private static string fileExtension = string.Empty;

        public UpdateImageController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public IActionResult Index(int id)  //открытие страницы редактирования изображения
        {
            var model = _unitOfWork.MediaRepo.GetById(id);

            nameImage = model.ImagePath;
            imagePath = Path.Combine(hostingEnvironment.WebRootPath, "uploads", model.ImagePath);

            byte[] imageArray = System.IO.File.ReadAllBytes(imagePath);
            imgBase64 = Convert.ToBase64String(imageArray);

            int indexDot = model.ImagePath.LastIndexOf('.');
            fileExtension = model.ImagePath.Substring(indexDot + 1);

            ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;

            return View();
        }

        private static byte[] BitmapToBytes(Bitmap img) //из Bitmap в байты
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }

        private static Bitmap Base64ToBimap(string base64) //из формата base64 в Bitmap
        {
            Bitmap bmpReturn = null;

            byte[] byteBuffer = Convert.FromBase64String(base64);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);

            memoryStream.Position = 0;
            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);

            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;

            return bmpReturn;
        }

        [HttpGet]
        public IActionResult UpdateImage(int brightness, int contrast)  //изменение яркости и контрастности
        {
            uint p;
            image = new Bitmap(Base64ToBimap(imgBase64));

            using (var fastBitmap = new FastBitmap(image))
            {
                pixel = new uint[fastBitmap.Width, fastBitmap.Height];

                for (int x = 0; x < fastBitmap.Width; x++)
                {
                    for (int y = 0; y < fastBitmap.Height; y++)
                    {
                        pixel[x, y] = (uint)(fastBitmap.GetPixel(x, y).ToArgb());
                    }                        
                }                    

                //яркость
                for (int i = 0; i < fastBitmap.Width; i++)
                {
                    for (int j = 0; j < fastBitmap.Height; j++)
                    {
                        p = BrightnessContrast.Brightness(pixel[i, j], brightness, 10); //получение значения цвета пискеля
                        fastBitmap.SetPixel(i, j, Color.FromArgb((int)p));  //присвоение полученного цвета пикселю на картинке
                        pixel[i, j] = (uint)(fastBitmap.GetPixel(i, j).ToArgb());  //занесение нового цвета пикселя в матрицу, т. е. сохранение
                    }
                }                    

                //контрастность
                for (int i = 0; i < fastBitmap.Width; i++)
                {
                    for (int j = 0; j < fastBitmap.Height; j++)
                    {
                        p = BrightnessContrast.Contrast(pixel[i, j], contrast, 10);
                        fastBitmap.SetPixel(i, j, Color.FromArgb((int)p));
                        pixel[i, j] = (uint)(fastBitmap.GetPixel(i, j).ToArgb());
                    }
                }                    
            }

            bitmapBytes = BitmapToBytes(image);
            image.Dispose();

            imgBase64 = Convert.ToBase64String(bitmapBytes);
            ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;

            return View("Index");
        }

        [HttpGet]
        public IActionResult UpdateColorImage(int blueRed, int purpleGreen, int yellowDarkBlue) //изменение цветового баланса
        {
            uint p;
            image = new Bitmap(Base64ToBimap(imgBase64));

            using (var fastBitmap = new FastBitmap(image))
            {
                //получение матрицы с пикселями
                pixel = new uint[fastBitmap.Width, fastBitmap.Height];

                for (int x = 0; x < fastBitmap.Width; x++)
                {
                    for (int y = 0; y < fastBitmap.Height; y++)
                    {
                        pixel[x, y] = (uint)(fastBitmap.GetPixel(x, y).ToArgb());
                    }                        
                }                    

                //цветовой баланс R
                for (int i = 0; i < fastBitmap.Width; i++)
                {
                    for (int j = 0; j < fastBitmap.Height; j++)
                    {
                        p = ColorBalance.ColorBalance_R(pixel[i, j], blueRed, 10);

                        fastBitmap.SetPixel(i, j, Color.FromArgb((int)p));
                        pixel[i, j] = (uint)(fastBitmap.GetPixel(i, j).ToArgb());
                    }
                }                    

                //цветовой баланс G
                for (int i = 0; i < fastBitmap.Width; i++)
                {
                    for (int j = 0; j < fastBitmap.Height; j++)
                    {
                        p = ColorBalance.ColorBalance_G(pixel[i, j], purpleGreen, 10);

                        fastBitmap.SetPixel(i, j, Color.FromArgb((int)p));
                        pixel[i, j] = (uint)(fastBitmap.GetPixel(i, j).ToArgb());
                    }
                }                    

                //цветовой баланс B
                for (int i = 0; i < fastBitmap.Width; i++)
                {
                    for (int j = 0; j < fastBitmap.Height; j++)
                    {
                        p = ColorBalance.ColorBalance_B(pixel[i, j], yellowDarkBlue, 10);

                        fastBitmap.SetPixel(i, j, Color.FromArgb((int)p));
                        pixel[i, j] = (uint)(fastBitmap.GetPixel(i, j).ToArgb());
                    }
                }                    
            }            

            bitmapBytes = BitmapToBytes(image);
            image.Dispose();

            imgBase64 = Convert.ToBase64String(bitmapBytes);
            ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;

            return View("Index");
        }

        [HttpGet]
        public IActionResult UpdateSharpnessImage() //повышение резкости
        {
            image = new Bitmap(Base64ToBimap(imgBase64));

            using (var fastBitmap = new FastBitmap(image))
            {
                pixel = new uint[fastBitmap.Width, fastBitmap.Height];

                //получение матрицы пикселей
                for (int x = 0; x < fastBitmap.Width; x++)
                {
                    for (int y = 0; y < fastBitmap.Height; y++)
                    {
                        pixel[x, y] = (uint)(fastBitmap.GetPixel(x, y).ToArgb());
                    }                        
                }                    

                pixel = Filter.matrix_filtration(fastBitmap.Width, fastBitmap.Height, pixel, Filter.N1, Filter.sharpness);

                //установка новых занчений пикселям
                for (int x = 0; x < fastBitmap.Width; x++)
                {
                    for (int y = 0; y < fastBitmap.Height; y++)
                    {
                        fastBitmap.SetPixel(x, y, Color.FromArgb((int)pixel[x, y]));
                    }                        
                }                    
            }                      

            bitmapBytes = BitmapToBytes(image);
            image.Dispose();

            imgBase64 = Convert.ToBase64String(bitmapBytes);
            ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;

            return View("Index");
        }

        [HttpGet]
        public IActionResult UpdateBlurImage()  //размытие
        {
            image = new Bitmap(Base64ToBimap(imgBase64));

            using (var fastBitmap = new FastBitmap(image))
            {
                pixel = new uint[fastBitmap.Width, fastBitmap.Height];

                for (int x = 0; x < fastBitmap.Width; x++)
                {
                    for (int y = 0; y < fastBitmap.Height; y++)
                    {
                        pixel[x, y] = (uint)(fastBitmap.GetPixel(x, y).ToArgb());
                    }                        
                }                    

                pixel = Filter.matrix_filtration(fastBitmap.Width, fastBitmap.Height, pixel, Filter.N2, Filter.blur);

                for (int x = 0; x < fastBitmap.Width; x++)
                {
                    for (int y = 0; y < fastBitmap.Height; y++)
                    {
                        fastBitmap.SetPixel(x, y, Color.FromArgb((int)pixel[x, y]));
                    }                        
                }                    
            }            

            bitmapBytes = BitmapToBytes(image);
            image.Dispose();

            imgBase64 = Convert.ToBase64String(bitmapBytes);
            ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;

            return View("Index");
        }

        [HttpGet]
        public IActionResult UpdateBlackWhiteImage()    //черно-белое изображение
        {
            image = new Bitmap(Base64ToBimap(imgBase64));

            using (var fastBitmap = new FastBitmap(image))
            {
                for (int x = 0; x < fastBitmap.Width; x++)
                {
                    for (int y = 0; y < fastBitmap.Height; y++)
                    {
                        // получаем (i, j) пиксель
                        uint pixel = (uint)(fastBitmap.GetPixel(x, y).ToArgb());

                        // получаем компоненты цветов пикселя
                        float R = (float)((pixel & 0x00FF0000) >> 16); // красный
                        float G = (float)((pixel & 0x0000FF00) >> 8); // зеленый
                        float B = (float)(pixel & 0x000000FF); // синий
                                                               // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                        R = G = B = (R + G + B) / 3.0f;

                        // собираем новый пиксель по частям (по каналам)
                        uint newPixel = 0xFF000000 | ((uint)R << 16) | ((uint)G << 8) | ((uint)B);

                        // добавляем его в Bitmap нового изображения
                        fastBitmap.SetPixel(x, y, Color.FromArgb((int)newPixel));
                    }
                }                    
            }

            bitmapBytes = BitmapToBytes(image);
            image.Dispose();

            imgBase64 = Convert.ToBase64String(bitmapBytes);
            ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;
            
            return View("Index");
        }

        [HttpGet]
        public IActionResult UpdateInvertImage()    //инверсия изображения
        {
            image = new Bitmap(Base64ToBimap(imgBase64));

            BitmapData bmData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            int stride = bmData.Stride;
            IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - image.Width * 4;
                int nWidth = image.Width;

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < nWidth; x++)
                    {
                        p[0] = (byte)(255 - p[0]); //red
                        p[1] = (byte)(255 - p[1]); //green
                        p[2] = (byte)(255 - p[2]); //blue

                        p += 4;
                    }
                    p += nOffset;
                }
            }
            image.UnlockBits(bmData);

            bitmapBytes = BitmapToBytes(image);
            image.Dispose();

            imgBase64 = Convert.ToBase64String(bitmapBytes);
            ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;

            return View("Index");
        }             

        [HttpGet]
        public IActionResult UpdateRotate90Image()  //поворот на 90
        {
            image = Base64ToBimap(imgBase64);

            image.RotateFlip(RotateFlipType.Rotate90FlipNone);

            bitmapBytes = BitmapToBytes(image);
            image.Dispose();

            imgBase64 = Convert.ToBase64String(bitmapBytes);
            ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;

            return View("Index");
        }

        [HttpGet]
        public IActionResult UpdateRotate180Image() //поворот на 180
        {
            image = new Bitmap(Base64ToBimap(imgBase64));

            image.RotateFlip(RotateFlipType.Rotate180FlipNone);

            bitmapBytes = BitmapToBytes(image);
            image.Dispose();

            imgBase64 = Convert.ToBase64String(bitmapBytes);
            ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;

            return View("Index");
        }

        [HttpGet]
        public IActionResult UpdateRotate270Image() //поворот на 270
        {
            image = new Bitmap(Base64ToBimap(imgBase64));

            image.RotateFlip(RotateFlipType.Rotate270FlipNone);

            bitmapBytes = BitmapToBytes(image);
            image.Dispose();

            imgBase64 = Convert.ToBase64String(bitmapBytes);
            ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;

            return View("Index");
        }

        [HttpGet]
        public IActionResult CropImage(float imageX, float imageY, float imageW, float imageH)  //получения данных кадрирования изображения
        {
            int x = Convert.ToInt32(imageX);
            int y = Convert.ToInt32(imageY);
            int w = Convert.ToInt32(imageW);
            int h = Convert.ToInt32(imageH);

            bitmapBytes = Cropping(x, y, w, h);

            imgBase64 = Convert.ToBase64String(bitmapBytes);
            ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;

            return View("Index");
        }

        private byte[] Cropping(int coordinateX, int coordinateY, int width, int height)    //кадрирование изображения
        {            
            try
            {
                using (Image originalImage = Base64ToBimap(imgBase64))
                {
                    using (Bitmap bmp = new Bitmap(width, height))
                    {
                        bmp.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);
                        using (Graphics graphics = Graphics.FromImage(bmp))
                        {
                            graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            graphics.DrawImage(originalImage, new Rectangle(0, 0, width, height), coordinateX, coordinateY, width, height, GraphicsUnit.Pixel);

                            MemoryStream ms = new MemoryStream();
                            bmp.Save(ms, originalImage.RawFormat);
                            return ms.GetBuffer();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        [HttpGet]
        public IActionResult AddWatermark(string textWatermark) //добавлени водяного знака изображению
        {
            using (Bitmap bitmap = Base64ToBimap(imgBase64))
            {
                using (Bitmap tempBitmap = new Bitmap(bitmap.Width, bitmap.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(tempBitmap))
                    {
                        graphics.DrawImage(bitmap, 0, 0);
                        bitmap.Dispose();

                        Brush brush = new SolidBrush(Color.FromArgb(128, 255, 255, 255)); //цвет текста
                        double fontSize = 30;
                        Font font = new Font(FontFamily.GenericSansSerif, (float)fontSize, FontStyle.Bold, GraphicsUnit.Pixel); //шрифт
                        SizeF textSize = graphics.MeasureString(textWatermark, font);

                        //установка размера текста в зависимости от размера изображения
                        while (((int)textSize.Width + 10) >= tempBitmap.Width)
                        {
                            if (fontSize != 0.5)
                            {
                                fontSize -= 0.5;
                            }                                
                            else
                            {
                                break;
                            }                                

                            font = new Font(FontFamily.GenericSansSerif, (float)fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
                            textSize = graphics.MeasureString(textWatermark, font);
                        }

                        Point position = new Point((tempBitmap.Width - ((int)textSize.Width + 10)),
                            (tempBitmap.Height - ((int)textSize.Height + 10)));

                        graphics.DrawString(textWatermark, font, brush, position);

                        MemoryStream ms = new MemoryStream();
                        Image originalImage = Base64ToBimap(imgBase64);
                        tempBitmap.Save(ms, originalImage.RawFormat);

                        bitmapBytes = ms.GetBuffer();
                        imgBase64 = Convert.ToBase64String(bitmapBytes);
                        ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;

                        return View("Index");
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult UpdateFlipXImage() //отражание по горизонтали
        {
            image = new Bitmap(Base64ToBimap(imgBase64));

            image.RotateFlip(RotateFlipType.RotateNoneFlipX);

            bitmapBytes = BitmapToBytes(image);
            image.Dispose();

            imgBase64 = Convert.ToBase64String(bitmapBytes);
            ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;

            return View("Index");
        }

        [HttpGet]
        public IActionResult UpdateFlipYImage() //отражение по вертикали
        {
            image = new Bitmap(Base64ToBimap(imgBase64));

            image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            bitmapBytes = BitmapToBytes(image);
            image.Dispose();

            imgBase64 = Convert.ToBase64String(bitmapBytes);
            ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;

            return View("Index");
        }

        [HttpGet]
        public IActionResult SaveImage()    //сохранение измененного изображения
        {
            try
            {
                string pathFile = Path.Combine(hostingEnvironment.WebRootPath, "update" + nameImage);
                System.IO.File.WriteAllBytes(pathFile, bitmapBytes);
                System.IO.File.Delete(imagePath);
                System.IO.File.Move(pathFile, imagePath);

                imgBase64 = Convert.ToBase64String(bitmapBytes);
                ViewData["imageLocation"] = "data:image/" + fileExtension + ";base64," + imgBase64;
                ViewData["savedUpdate"] = "true";

                return View("Index");
            }
            catch 
            {
                ViewData["savedUpdate"] = "false";

                return View("Index");
            }
        }
    }
}

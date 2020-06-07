using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageGallery.AdditionalClass
{
    public unsafe class FastBitmap : IDisposable
    {
        delegate void SetPixelInternal(int x, int y, Color clr);
        delegate Color GetPixelInternal(int x, int y);

        Bitmap _bmp;
        BitmapData _bd;
        bool _locked;
        byte* pStart;

        SetPixelInternal setPix;
        GetPixelInternal getPix;

        public FastBitmap(Bitmap bmp, bool @lock)
        {
            _bmp = bmp;

            if (@lock)
            {
                Lock();
            }                
        }

        public FastBitmap(Bitmap bmp) : this(bmp, true)
        {

        }

        public void Lock()
        {
            if (!_locked)
            {
                _bd = _bmp.LockBits(new Rectangle(0, 0, _bmp.Width, _bmp.Height), ImageLockMode.ReadWrite, _bmp.PixelFormat);

                pStart = (byte*)_bd.Scan0;
                _locked = true;

                switch (_bmp.PixelFormat)
                {
                    case PixelFormat.Format24bppRgb:
                        setPix = SetPixel24;
                        getPix = GetPixel24;
                        break;
                    case PixelFormat.Format32bppArgb:
                        setPix = SetPixel32;
                        getPix = GetPixel32;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public void Unlock()
        {
            if (!_locked)
            {
                return;
            }                

            _bmp.UnlockBits(_bd);

            getPix = null;
            setPix = null;
            _locked = false;
        }

        public Bitmap Bitmap
        {
            get
            {
                return (_locked ? null : _bmp);
            }
            set
            {
                if (_locked)
                {
                    throw new Exception("Image locked!");
                }
                    
                if (value == null)
                {
                    throw new NullReferenceException();
                }                    

                _bmp = value;
            }
        }

        public int Width
        {
            get
            {
                return _bmp.Width;
            }
        }

        public int Height
        {
            get
            {
                return _bmp.Height;
            }
        }

        public void SetPixel(int x, int y, Color clr)
        {
            if (!_locked)
            {
                throw new Exception();
            }                

            setPix(x, y, clr);
        }

        void SetPixel24(int x, int y, Color clr)
        {
            var pMem = pStart + x * 3 + y * _bd.Stride;

            *pMem = clr.B;
            *(pMem + 1) = clr.G;
            *(pMem + 2) = clr.R;
        }

        void SetPixel32(int x, int y, Color clr)
        {
            var pMem = pStart + x * 4 + y * _bd.Stride;

            *pMem = clr.B;
            *(pMem + 1) = clr.G;
            *(pMem + 2) = clr.R;
            *(pMem + 3) = clr.A;
        }

        public Color GetPixel(int x, int y)
        {
            if (!_locked)
            {
                throw new Exception();
            }                

            return getPix(x, y);
        }

        Color GetPixel24(int x, int y)
        {
            var pMem = pStart + x * 3 + y * _bd.Stride;

            return Color.FromArgb(*(pMem + 2), *(pMem + 1), *pMem);
        }

        Color GetPixel32(int x, int y)
        {
            var pMem = pStart + x * 4 + y * _bd.Stride;

            return Color.FromArgb(*(pMem + 3), *(pMem + 2), *(pMem + 1), *pMem);
        }

        public bool IsLocked
        {
            get
            {
                return _locked;
            }
        }

        public void Dispose()
        {
            if (_locked)
            {
                Unlock();
            }                
        }
    }
}

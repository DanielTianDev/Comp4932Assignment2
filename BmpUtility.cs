using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Comp4932_Assignment2
{
    class BmpUtility
    { 
        public BmpUtility() { }

        public static byte[] consolidateData(double[,] A, double[,] B, double[,] C)
        {

            int size = (A.GetLength(0) * A.GetLength(1)) * 3;
            int rows = A.GetLength(0);
            int columns = A.GetLength(1);

            byte[] result = new byte[size];

            int i = 0;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    result[i++] = (byte)A[c, r];
                    result[i++] = (byte)B[c, r];
                    result[i++] = (byte)C[c, r];
                }
            }

            return result;
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }


        public static void writeFile()
        {
            String filePath = "";
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.OverwritePrompt = true;
            //saveFile.FileName = "bmp";
            saveFile.DefaultExt = "bmp";
            saveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            DialogResult result = saveFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                filePath = saveFile.FileName;
            }

            try
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                // Use BinaryWriter to write the bytes to the file
                BinaryWriter writer = new BinaryWriter(fileStream);

                /*
                // Write the header
                writer.Write(Header.ChunkID);
                writer.Write(Header.ChunkSize);
                writer.Write(Header.Format);

                // Write the format chunk
                writer.Write(Header.Subchunk1ID);
                writer.Write(Header.Subchunk1Size);
                writer.Write(Header.AudioFormat);
                writer.Write(Header.NumChannels);
                writer.Write(Header.SampleRate);
                writer.Write(Header.ByteRate);
                writer.Write(Header.BlockSize);
                writer.Write(Header.BitsPerSample);

                //Write the data chunk
                writer.Write(Header.Data);
                writer.Write(Header.DataSize);

                switch (Header.NumChannels)
                {
                    case 1:
                        switch (Header.BitsPerSample)
                        {
                            case 8:
                                foreach (byte sampleValue in samples)
                                {
                                    writer.Write(sampleValue);
                                }
                                break;
                            case 16:
                                foreach (short sampleValue in samples)
                                {
                                    writer.Write(sampleValue);
                                }
                                break;
                            case 32:
                                foreach (int sampleValue in samples)
                                {
                                    writer.Write(sampleValue);
                                }
                                break;
                        }
                        break;
                    case 2:
                        for (int i = 0; i < samples.Length; i++)
                        {
                            writer.Write((short)samples[i]);
                            writer.Write((short)rightSamples[i]);
                        }
                        break;
                    default:
                        MessageBox.Show("Sorry, this program currently only supports mono and stereo audio.");
                        break;
                }

                 */
                fileStream.Close();
                writer.Close();
            }
            catch (Exception ex)
            {
                if (ex != null)
                    MessageBox.Show("An error occured creating your file " + ex.Message);
            }

        }

        /// <summary>
        /// function CopyDataToBitmap
        /// Purpose: Given the pixel data return a bitmap of size [352,288],PixelFormat=24RGB 
        /// </summary>
        /// <param name="data">Byte array with pixel data</param>
        public Bitmap CopyDataToBitmap(byte[] data)
        {
            //Here create the Bitmap to the know height, width and format
            Bitmap bmp = new Bitmap(512, 512, PixelFormat.Format24bppRgb);

            //Create a BitmapData and Lock all pixels to be written 
            BitmapData bmpData = bmp.LockBits(
                                 new Rectangle(0, 0, bmp.Width, bmp.Height),
                                 ImageLockMode.WriteOnly, bmp.PixelFormat);

            //Copy the data from the byte array into BitmapData.Scan0
            Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
            //Unlock the pixels
            bmp.UnlockBits(bmpData);
            //Return the bitmap 
            return bmp;
        }


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Comp4932_Assignment2
{


    /**
        Assignment requires 2 pictuers to be loaded in
        1st picture: i frame, its just a jpeg 

        second image: using motion vectors, and then applying the jpeg steps.

        leave ycbcr as float, don't leave out any information
        only when u quantize, then push into a byte

        PFRAME relies on the iframe. if iframe is not present, then the pframe wil lfail.
        pframes only require one FRAME of reference. 

    */

    public partial class Main : Form
    {

        string selectedFilePath, selectedFileName;

        public static int[,] luminanceQuantizationTable =
        {
            { 16,  11,  10,  16,  24,  40,  51,  61 },
            {12,  12,  14,  19,  26,  58,  60,  55  },
            {14,  13,  16,  24,  40,  57,  69,  56  },
            {14,  17,  22,  29,  51,  87,  80,  62  },
            {18,  22,  37,  56,  68, 109, 103,  77  },
            {24,  35,  55,  64,  81, 104, 113,  92  },
            {49,  64,  78,  87, 103, 121, 120,  101 },
            {72,  92,  95,  98, 112, 100, 103,  99  }
        };

        public static int[,] chrominanceQuantizationTable =
        {
            {17, 18, 24, 47, 99, 99, 99, 99},
            {18, 21, 26, 66, 99, 99, 99, 99},
            {24, 26, 56, 99, 99, 99, 99, 99},
            {47, 66, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99}
        };

        public static int[] zigZagReverse = { 0, 1, 5, 6,14,15,27,28, 2, 4, 7,13,16,26,29,42, 3, 8,12,17,25,30,41,43,
                                            9,11,18,24,31,40,44,53, 10,19,23,32,39,45,52,54, 20,22,33,38,46,51,55,60,
                                            21,34,37,47,50,56,59,61, 35,36,48,49,57,58,62,63};


        public static int[,] zigzagMatrix = { { 0, 1, 5, 6, 14, 15, 27, 28 },
                                              { 2, 4, 7,13,16,26,29,42},
                                               {3, 8,12,17,25,30,41,43},
                                               {9,11,18,24,31,40,44,53},
                                              {10,19,23,32,39,45,52,54},
                                              {20,22,33,38,46,51,55,60},
                                              {21,34,37,47,50,56,59,61},
                                              {35,36,48,49,57,58,62,63}  };


        public Main()
        {
            InitializeComponent();
            Size = new Size(1500, 900);

            test();

        }

        public void writeFile(string fileName, List<int> data)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {

                writer.Write(data[data.Count - 5]);
                writer.Write(data[data.Count - 4]);
                writer.Write(data[data.Count - 3]);
                writer.Write(data[data.Count - 2]);
                writer.Write(data[data.Count - 1]);

                for (int i = 0; i < data.Count - 3; i++)
                {
                    /*if (data[i] > 255)
                    {
                        writer.Write((byte)255);
                    }
                    else
                    {
                        writer.Write(data[i]);
                    }
                    */

                    int temp = data[i];
                    sbyte temp2 = (sbyte)temp;

                    if (temp > 127) temp2 = 127;

                    writer.Write(temp2);
                }

                writer.Close();
            }
        }

        public void readFile(string fileName, List<int> readData)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {

                readData.Add(reader.ReadInt32());
                readData.Add(reader.ReadInt32());
                readData.Add(reader.ReadInt32());
                readData.Add(reader.ReadInt32());
                readData.Add(reader.ReadInt32());
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    //readData.Add(reader.ReadByte());
                    readData.Add(reader.ReadSByte());
                }

                reader.Close();
            }
        }


        public string format(double[,] data)
        {
            string result = "";

            for (int r = 0; r < data.GetLength(0); r++)
            {
                for (int c = 0; c < data.GetLength(1); c++)
                {
                    result += Math.Round(data[r, c]) + " , ";
                }
                result += "\n";
            }
            return result;
        }

        public string format(int[,] data)
        {
            string result = "";

            for (int r = 0; r < data.GetLength(0); r++)
            {
                for (int c = 0; c < data.GetLength(1); c++)
                {
                    result += (data[r, c]) + " , ";
                }
                result += "\n";
            }
            return result;
        }

        public void test()
        {

            int[,] data ={ { 200, 202, 189, 188, 189, 175, 175, 175 },
                            { 200, 203, 198, 188, 189, 182, 178, 175},
                            {203, 200, 200, 195, 200, 187, 185, 175},
                            {200, 200, 200, 200, 197, 187, 187, 187},
                            {200, 205, 200, 200, 195, 188, 187, 175},
                            {200, 200, 200, 200, 200, 190, 187, 175},
                            {205, 200, 199, 200, 191, 187, 187, 175 },
                            {210, 200, 200, 200, 188, 185, 187, 186} };

            int[,] data2 ={ {70, 70, 100, 70, 87, 87, 150, 187 },
                            {85, 100, 96, 79, 87, 154, 87, 113},
                            {100, 85, 116, 79, 70, 87, 86, 196},
                            {136, 69, 87, 200, 79, 71, 117, 96},
                            {161, 70, 87, 200, 103, 71, 96, 113},
                            {161, 123, 147, 133, 113, 113, 85, 161},
                            {146, 147, 175, 100, 103, 103, 163, 187},
                            {156, 146, 189, 70, 113, 161, 163, 197} };


            int[,] data4 ={ { 200, 202, 189, 188, 189, 175, 175, 175 , 70, 70, 100, 70, 87, 87, 150, 187, 69 },
                            { 200, 203, 198, 188, 189, 182, 178, 175 , 85, 100, 96, 79, 87, 154, 87, 113,  69},
                            {203, 200, 200, 195, 200, 187, 185, 175 , 100, 85, 116, 79, 70, 87, 86, 196,  69},
                            {200, 200, 200, 200, 197, 187, 187, 187, 136, 69, 87, 200, 79, 71, 117, 96 , 69},
                            {200, 205, 200, 200, 195, 188, 187, 175, 161, 70, 87, 200, 103, 71, 96, 113 , 55},
                            {200, 200, 200, 200, 200, 190, 187, 175, 161, 123, 147, 133, 113, 113, 85, 161 , 34},
                            {205, 200, 199, 200, 191, 187, 187, 175 ,146, 147, 175, 100, 103, 103, 163, 187, 23},
                            {210, 200, 200, 200, 188, 185, 187, 186 , 156, 146, 189, 70, 113, 161, 163, 197, 41},

                            {200, 202, 189, 188, 189, 175, 175, 175 , 70, 70, 100, 70, 87, 87, 150, 187 ,50},
                            { 200, 203, 198, 188, 189, 182, 178, 175 , 85, 100, 96, 79, 87, 154, 87, 113 ,55},
                            {203, 200, 200, 195, 200, 187, 185, 175 , 100, 85, 116, 79, 70, 87, 86, 196 ,96},
                            {200, 200, 200, 200, 197, 187, 187, 187, 136, 69, 87, 200, 79, 71, 117, 96, 66},
                            {200, 205, 200, 200, 195, 188, 187, 175, 161, 70, 87, 200, 103, 71, 96, 113 , 420},
                            {200, 200, 200, 200, 200, 190, 187, 175, 161, 123, 147, 133, 113, 113, 85, 161, 123},
                            {205, 200, 199, 200, 191, 187, 187, 175 ,146, 147, 175, 100, 103, 103, 163, 187, 321},
                            {210, 200, 200, 200, 188, 185, 187, 186 , 156, 146, 189, 70, 113, 161, 163, 197 , 456 },
                            {420, 200, 200, 200, 188, 185, 187, 186 , 156, 146, 189, 70, 113, 161, 163, 197 , 1337 },
                            {888, 69, 69, 69, 70, 185, 69, 69 , 39, 69, 69, 70, 113, 161, 163, 420 , 666 }};

            int[,] data5 ={ { 200, 202, 189, 188, 189, 175, 175, 175 , 70, 70, 100, 70, 87, 87, 150, 187, 69 },
                            { 200, 203, 198, 188, 189, 182, 178, 175 , 85, 100, 96, 79, 87, 154, 87, 113,  69},
                            {203, 200, 200, 195, 200, 187, 185, 175 , 100, 85, 116, 79, 70, 87, 86, 196,  69},
                            {200, 200, 200, 200, 197, 187, 187, 187, 136, 69, 87, 200, 79, 71, 117, 96 , 69},
                            {200, 205, 200, 200, 195, 188, 187, 175, 161, 70, 87, 200, 103, 71, 96, 113 , 55},
                            {200, 200, 200, 200, 200, 190, 187, 175, 161, 123, 147, 133, 113, 113, 85, 161 , 34},
                            {205, 200, 199, 200, 191, 187, 187, 175 ,146, 147, 175, 100, 103, 103, 163, 187, 23},
                            {210, 200, 200, 200, 188, 185, 187, 186 , 156, 146, 189, 70, 113, 161, 163, 197, 41},

                            {200, 202, 189, 188, 189, 175, 175, 175 , 70, 70, 100, 70, 87, 87, 150, 187 ,50},
                            { 200, 203, 198, 188, 189, 182, 178, 175 , 85, 100, 96, 79, 87, 154, 87, 113 ,55},
                            {203, 200, 200, 195, 200, 187, 185, 175 , 100, 85, 116, 79, 70, 87, 86, 196 ,96},
                            {200, 200, 200, 200, 197, 187, 187, 187, 136, 69, 87, 200, 79, 71, 117, 96, 66},
                            {200, 205, 200, 200, 195, 188, 187, 175, 161, 70, 87, 200, 103, 71, 96, 113 , 420},
                            {200, 200, 200, 200, 200, 190, 187, 175, 161, 123, 147, 133, 113, 113, 85, 161, 123},
                            {205, 200, 199, 200, 191, 187, 187, 175 ,146, 147, 175, 100, 103, 103, 163, 187, 321},
                            {210, 200, 200, 200, 188, 185, 187, 186 , 156, 146, 189, 70, 113, 161, 163, 197 , 456 },
                            {420, 200, 200, 200, 188, 185, 187, 186 , 156, 146, 189, 70, 113, 161, 163, 197 , 1337 }};



            double[,] fuck = forwardDCT(data);
            string haha = format(fuck);

           // List<int> YRunEncoded = new List<int>();
            //eightByEightSampling(data4, YRunEncoded, true);
            //int[,] trap = reconstructSingleChannel(YRunEncoded, 17, 18, true);



            //string trap2 = format(trap);


            /*            

             int[] values = new int[64];
            int counter = 0;
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    values[counter++] = bob[r, c];

                }
            }

            List<int> rle = RLE(values);

            int[] decoded = decodeRLE(rle);*/

            // double[,] Fuv = forwardDCT(data);

            //int[,] quantizedUv = quantize(Fuv, luminanceQuantizationTable);

            //int[,] bob = ZigZag(8, data5, zigzagMatrix); //forward zigzag

            //int[,] reversedZigZag = reverseZigZag(8,  values, zigZagReverse);

            //string resultReverse = format(reversedZigZag);

            //eightByEightSampling(data5);

            /*
            double[,] Fuv = forwardDCT(data);


            int[,] quantizedUv = quantize(Fuv, luminanceQuantizationTable);

            int[,] reverseQuantizedUv = reverseQuantize(quantizedUv, luminanceQuantizationTable);

            double[,] reversedDctFuv = reverseDct(reverseQuantizedUv);

            string resultFormat = format(quantizedUv);*/

        }

        public int[] twoDToOneD(int[,] matrix)
        {
            int[] result = new int[64];
            int counter = 0;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    result[counter++] = matrix[r, c];

                }
            }

            return result;
        }

        public List<int> RLE(int[] values)
        {
            List<int> encoded = new List<int>();

            encoded.Add(values[0]);

            int runLength = 0;

            for (int i = 1; i < values.Length; i++) //if only one 0, then should be 0, 1 
            {


                if (values[i] != 0)
                {
                    encoded.Add(runLength);
                    encoded.Add(values[i]);
                    runLength = 0;
                }
                else
                {
                    runLength++;
                }
            }

            encoded.Add(0);
            encoded.Add(0);

            return encoded;
        }


        public int[] decodeRLE(List<int> values)
        {
            int[] decoded = new int[64];
            if (values[0] == 0 && values[1] == 0) return decoded;

            int counter = 1;
            decoded[0] = values[0];
            int i;

            try
            {
                for (i = 1; i < values.Count; i += 2)
                {
                    if (values[i] == 0 && values[i + 1] == 0)
                    {
                        break;
                    }

                    int length = values[i];

                    for (int q = 0; q < length; q++)
                    {
                        decoded[counter++] = 0;
                    }
                    decoded[counter++] = values[i + 1];

                }

            }
            catch(Exception e)
            {
                string konosuba = e.Message;
            }
            



            return decoded;
        }

        public static int calculateZigZag8Row(int position)
        {
            int r = (int)Math.Floor((double)(position / 8));
            return (int)Math.Floor((double)(position / 8));
        }

        public static int calculateZigZag8Column(int position)
        {
            int r = position % 8;
            return position % 8;
        }




        public static int[,] reverseZigZag(int n, int[] values, int[] zigzagmap)
        {
            int[,] result = new int[n, n];

            int i = 0;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    result[r, c] = values[zigzagmap[i++]];

                }
            }


            return result;
        }

        public static int[,] ZigZag(int n, int[,] quantizedSamples, int[,] zigzagmap)
        {
            int[,] result = new int[n, n];

            int tempCounter = 0;
            int resultR = 0, resultC = 0;

            while (tempCounter < 64)
            {
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 8; c++)
                    {

                        if (tempCounter == zigzagmap[r, c])
                        {
                            tempCounter++;

                            if (resultC >= 8)
                            {
                                resultC = 0;
                                resultR++;
                            }

                            result[resultR, resultC++] = quantizedSamples[r, c];


                        }
                    }
                }
            }

            return result;
        }

        private double C(int x)
        {
            if (x == 0)
            {
                return Math.Sqrt(2) / 2;
            }
            else { // it is not zero
                return 1;
            }
        }


        //page 244
        public double[,] forwardDCT(int[,] pixels)
        {
            double[,] forwardData = new double[8, 8];
            double temp = 0;
            for (int u = 0; u < 8; u++)
            {
                for (int v = 0; v < 8; v++)
                {
                    temp = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            temp += Math.Cos(((2 * i + 1) * u * Math.PI) / 16)
                                * Math.Cos(((2 * j + 1) * v * Math.PI) / 16)
                                * pixels[i, j];
                        }
                    }

                    forwardData[u, v] = ((temp * ((C(u) * C(v)) / 4)));
                }
            }
            return forwardData;
        }


        public double[,] reverseDct(int[,] F)
        {
            double[,] reverseResult = new double[8, 8];
            double temp = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    temp = 0;
                    for (int u = 0; u < 8; u++)
                    {
                        for (int v = 0; v < 8; v++)
                        {
                            temp += ((C(u) * C(v)) / 4) * Math.Cos(((2 * i + 1) * u * Math.PI) / 16)
                                * Math.Cos(((2 * j + 1) * v * Math.PI) / 16)
                                * F[u, v];
                        }
                    }
                    reverseResult[i, j] = Math.Round(temp);
                }
            }
            return reverseResult;
        }


        public int[,] quantize(double[,] samples, int[,] qTable)
        {
            int[,] quantizedResults = new int[8, 8];

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    quantizedResults[r, c] = (int)Math.Round(samples[r, c] / qTable[r, c]);
                }
            }

            return quantizedResults;
        }

        public int[,] reverseQuantize(int[,] samples, int[,] qTable)
        {
            int[,] quantizedResults = new int[8, 8];

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    quantizedResults[r, c] = samples[r, c] * qTable[r, c];
                }
            }

            return quantizedResults;
        }



        public int[,] reconstructSingleChannel(List<int> input, int imgWidth, int imgHeight, bool isLuminance)
        {

            int[,] output = new int[imgHeight, imgWidth];

            int blocksHorizontal = (int)Math.Ceiling((double)imgWidth / 8);

            int blocksVertical = (int)Math.Ceiling((double)imgHeight / 8);

            int blockCount = blocksHorizontal * blocksVertical;

            List<int[]> resultStorage = new List<int[]>();  //stores the entire reconstructed rle data, still needs further decoding

            List<int> tempStorage = new List<int>();    //temp list to get each 8x8 block from rle
            int[] tempValues;
            int rleCounter = 0;


            for (int i = 0; i < blockCount; i++)
            {
                tempValues = new int[64];

                for (; rleCounter < input.Count; rleCounter++)
                {
                    if (input[rleCounter] == 0 && input[rleCounter + 1] == 0)
                    {
                        tempStorage.Add(0);
                        tempStorage.Add(0);
                        rleCounter += 2;
                        tempValues = decodeRLE(tempStorage);
                        resultStorage.Add(tempValues);
                        break;
                    }
                    tempStorage.Add(input[rleCounter]);
                        
                }

                tempStorage.Clear();
            }

            //reverseZigZag

            int rowIndex, colIndex, blockCounter = 0;

            for (int r = 0; r < output.GetLength(0); r += 8) //rows
            {
                for (int c = 0; c < output.GetLength(1); c += 8) //columns
                {

                    int[,] inverseZigZaggedsamples = reverseZigZag(8, resultStorage[blockCounter++], zigZagReverse);    //reverse zig zag
                    int[,] frequencySamples;
                    if (isLuminance)//reverse quantize
                    {
                        frequencySamples = reverseQuantize(inverseZigZaggedsamples, luminanceQuantizationTable);  //Y
                    }
                    else  //Cb or Cr
                    {
                        frequencySamples = reverseQuantize(inverseZigZaggedsamples, chrominanceQuantizationTable);
                        //frequencySamples = reverseQuantize(inverseZigZaggedsamples, luminanceQuantizationTable);
                    }

                    double[,] samples = reverseDct(frequencySamples);

                    for (rowIndex = 0; rowIndex < 8; rowIndex++)
                    {
                        for (colIndex = 0; colIndex < 8; colIndex++)
                        {

                            if (rowIndex + r >= output.GetLength(0))
                            {
                                continue;
                            }else if(colIndex + c >= output.GetLength(1))
                            {
                                continue;
                            }
                            else
                            {
                                output[rowIndex + r, colIndex + c] = (int)samples[rowIndex, colIndex];
                            }                     
                        }
                    }
                }
            }


            return output;
        }


        public void eightByEightSampling(int[,] samples, List<int> output, bool isLuminance)
        {

            int rowIndex, colIndex;
            int[,] result;

            for (int r = 0; r < samples.GetLength(0); r += 8) //rows
            {
                for (int c = 0; c < samples.GetLength(1); c += 8) //columns
                {
                    result = new int[8, 8];

                    for (rowIndex = 0; rowIndex < 8; rowIndex++)
                    {
                        for (colIndex = 0; colIndex < 8; colIndex++)
                        {

                            if (colIndex + c >= samples.GetLength(1))
                            {
                                result[rowIndex, colIndex] = 0;

                            }
                            else if (rowIndex + r >= samples.GetLength(0))
                            {
                                result[rowIndex, colIndex] = 0;
                            }
                            else
                            {
                                result[rowIndex, colIndex] = samples[rowIndex + r, colIndex + c];
                            }

                        }
                    }

                    double[,] Fuv = forwardDCT(result);  //2D Discrete Fourier Transform step on 8x8 block
                    int[,] quantizedUv;
                    if (isLuminance)
                    {
                        quantizedUv = quantize(Fuv, luminanceQuantizationTable); //quantization step
                    }
                    else
                    {
                        quantizedUv = quantize(Fuv, chrominanceQuantizationTable); //quantization step
                    }

                    int[,] zigZaggedQUv = ZigZag(8, quantizedUv, zigzagMatrix);
                    List<int> temp = RLE(twoDToOneD(zigZaggedQUv));
                    output.AddRange(temp);
                }
            }
        }


        public void eightByEightSampling(double[,] samples, List<int> output, bool isLuminance)
        {

            int rowIndex, colIndex;

            int[,] result;

            for (int r = 0; r < samples.GetLength(0); r += 8) //rows
            {
                for (int c = 0; c < samples.GetLength(1); c += 8) //columns
                {
                    result = new int[8, 8];

                    for (rowIndex = 0; rowIndex < 8; rowIndex++)
                    {
                        for (colIndex = 0; colIndex < 8; colIndex++)
                        {

                            if (colIndex + c >= samples.GetLength(1))
                            {
                                result[rowIndex, colIndex] = 0;

                            }
                            else if (rowIndex + r >= samples.GetLength(0))
                            {
                                result[rowIndex, colIndex] = 0;
                            }
                            else
                            {
                                result[rowIndex, colIndex] = (int)Math.Round(samples[rowIndex + r, colIndex + c]);
                            }

                        }
                    }
                    
                    double[,] Fuv = forwardDCT(result);  //2D Discrete Fourier Transform step on 8x8 block
                    int[,] quantizedUv;
                    if (isLuminance)
                    {
                        quantizedUv = quantize(Fuv, luminanceQuantizationTable); //quantization step
                    }
                    else
                    {
                        quantizedUv = quantize(Fuv, chrominanceQuantizationTable); //quantization step
                    }                  
                    int[,] zigZaggedQUv = ZigZag(8, quantizedUv, zigzagMatrix);
                    List<int> temp = RLE(twoDToOneD(zigZaggedQUv));
                    output.AddRange(temp);

                }
            }


        }



        public List<int> compressImage(double[,] Y, double[,] subsampledCb, double[,] subsampledCr, int imgWidth, int imgHeight)
        {

            List<int> YRunEncoded = new List<int>();
            List<int> CbRunEncoded = new List<int>();
            List<int> CrRunEncoded = new List<int>();

            eightByEightSampling(Y, YRunEncoded, true);
            eightByEightSampling(subsampledCb, CbRunEncoded, false);
            eightByEightSampling(subsampledCr, CrRunEncoded, false);

            List<int> master = new List<int>();
            master.AddRange(YRunEncoded);
            master.AddRange(CbRunEncoded);
            master.AddRange(CrRunEncoded);

            master.Add(YRunEncoded.Count);  //header info
            master.Add(CbRunEncoded.Count);
            master.Add(CrRunEncoded.Count);
            master.Add(imgWidth);
            master.Add(imgHeight);

            return master;
        }

        public Bitmap decompressImage(List<int> master, int Ycount, int CbCount, int CrCount, int width, int height)
        {
            //decompress
            List<int> YRunDecoded = new List<int>();
            List<int> CbRunDecoded = new List<int>();
            List<int> CrRunDecoded = new List<int>();

            int counter = 5;
            for (int i = 0; i < Ycount; i++)
            {
                YRunDecoded.Add(master[counter++]);
            }

            for (int i = 0; i < CbCount; i++)
            {
                CbRunDecoded.Add(master[counter++]);
            }

            for (int i = 0; i < CrCount; i++)
            {
                CrRunDecoded.Add(master[counter++]);
            }

            int[,] Y = reconstructSingleChannel(YRunDecoded, height, width, true);
            int[,] subsampledCb = reconstructSingleChannel(CbRunDecoded, height / 2, width / 2, false);
            int[,] subsampledCr = reconstructSingleChannel(CrRunDecoded, height / 2, width / 2, false);


            Bitmap subsampledBitmap = reverseSubsample(Y, subsampledCb, subsampledCr); //rgb inverse

            for (int x = 0; x < subsampledBitmap.Width; x++)
            {
                for (int y = 0; y < subsampledBitmap.Height; y++)
                {
                    subsampledBitmap.SetPixel(x, y, GetColorFromYCbCr(subsampledBitmap.GetPixel(x, y).R, subsampledBitmap.GetPixel(x, y).G, subsampledBitmap.GetPixel(x, y).B));
                }
            }

            return subsampledBitmap;
        }



        public void loadImage(string filePath)
        {
            try
            {
                if (filePath == null) return;

                Image img = Image.FromFile(filePath);
                Bitmap mapOfBits = new Bitmap(img);

                //byte[] datas2 = ImageToByteArray(mapOfBits); File.WriteAllBytes("daniel.jpg", datas2);

                imgDisplay.Image = img; //sets initial image

                double[,] Y = null;
                double[,] Cb = null;
                double[,] Cr = null;

                setYCbCr(mapOfBits, ref Y, ref Cb, ref Cr);

                Bitmap YCbCr_Map = new Bitmap(img.Width, img.Height);

                for (int x = 0; x < mapOfBits.Width; x++)
                {
                    for (int y = 0; y < mapOfBits.Height; y++)
                    {
                        //Color temp = mapOfBits.GetPixel(x, y);
                        //YCbCr_Map.SetPixel(x, y, toY(temp));
                        YCbCr_Map.SetPixel(x, y, Color.FromArgb((byte)Y[x, y], (byte)Cb[x, y], (byte)Cr[x, y]));
                    }
                }

                imgIntermediate.Image = YCbCr_Map;

                double[,] subsampledCb = subSample(Cb);
                double[,] subsampledCr = subSample(Cr);
                List<int> rled = compressImage(Y, subsampledCb, subsampledCr, mapOfBits.Width, mapOfBits.Height);
                writeFile("baka.kawaii", rled);

                List<int> readData = new List<int>();
                readFile("baka.kawaii", readData);

                Bitmap subsampledBitmap = decompressImage(readData, readData[0], readData[1], readData[2], readData[3], readData[4]);

                imgReverse.Image = subsampledBitmap;

                byte[] datas = ImageToByteArray(subsampledBitmap); File.WriteAllBytes("daniel2.jpg", datas);

            }
            catch (Exception e)
            {
                if (e.Message != null)
                {
                    MessageBox.Show("An exception has occured: " + e.Message);
                }
                else
                {
                    MessageBox.Show("An exception has occured whilst loading an image.");
                }
            }
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public double[,] subSample(double[,] samples)
        {
            int subSampledRows = (samples.GetLength(0) % 2 == 0) ? samples.GetLength(0) / 2 : (int)Math.Round((samples.GetLength(0) / 2) + 0.5);
            int subSampledColumns = (samples.GetLength(1) % 2 == 0) ? samples.GetLength(1) / 2 : (int)Math.Round((samples.GetLength(1) / 2) + 0.5);

            double[,] result = new double[subSampledRows, subSampledColumns];

            int resultRow = 0, resultCol = 0;

            for (int row = 0; row < subSampledRows; row++)
            {
                resultCol = 0;

                for (int col = 0; col < subSampledColumns; col++)
                {
                    result[row, col] = samples[resultRow, resultCol];
                    resultCol += 2;
                }
                resultRow += 2;
            }

            return result;
        }



        public void setYCbCr(Bitmap rgb, ref double[,] Y, ref double[,] Cb, ref double[,] Cr)
        {

            Y = new double[rgb.Width, rgb.Height];
            Cb = new double[rgb.Width, rgb.Height];
            Cr = new double[rgb.Width, rgb.Height];

            for (int x = 0; x < rgb.Width; x++)
            {
                for (int y = 0; y < rgb.Height; y++)
                {
                    byte R = rgb.GetPixel(x, y).R;
                    byte G = rgb.GetPixel(x, y).G;
                    byte B = rgb.GetPixel(x, y).B;

                    Y[x, y] = (0 + (0.299 * R) + (0.587 * G) + (0.114 * B));
                    Cb[x, y] = (128 - (0.168736 * R) - (0.331264 * G) + (0.5 * B));
                    Cr[x, y] = (128 + (0.5 * R) - (0.418688 * G) - (0.081312 * B));

                }
            }
        }



        //http://stackoverflow.com/questions/4041840/function-to-convert-ycbcr-to-rgb
        Color GetColorFromYCbCr(int y, int cb, int cr)
        {
            double Y = y;
            double Cb = cb;
            double Cr = cr;

            int r = (int)(Y + 1.40200 * (Cr - 0x80));
            int g = (int)(Y - 0.34414 * (Cb - 0x80) - 0.71414 * (Cr - 0x80));
            int b = (int)(Y + 1.77200 * (Cb - 0x80));

            r = Math.Max(0, Math.Min(255, r));
            g = Math.Max(0, Math.Min(255, g));
            b = Math.Max(0, Math.Min(255, b));

            return Color.FromArgb(r, g, b);
        }


        /*
        public void backToRGB(double[,] Y, double[,] Cb, double[,] Cr
            , ref byte[,] R, ref byte[,] G, ref byte[,] B)
        {

            R = new byte[Y.GetLength(0), Y.GetLength(1)];
            G = new byte[Y.GetLength(0), Y.GetLength(1)];
            B = new byte[Y.GetLength(0), Y.GetLength(1)];

            for (int x = 0; x < Y.GetLength(0); x++)
            {
                for (int y = 0; y < Y.GetLength(1); y++)
                {
                    byte Y = yCbCr.GetPixel(x, y).R;
                    byte Cb = yCbCr.GetPixel(x, y).G;
                    byte Cr = yCbCr.GetPixel(x, y).B;
                  

                    Y[x, y] = (0 + (0.299 * R) + (0.587 * G) + (0.114 * B));
                    Cb[x, y] = (128 - (0.168736 * R) - (0.331264 * G) + (0.5 * B));
                    Cr[x, y] = (128 + (0.5 * R) - (0.418688 * G) - (0.081312 * B));

                }
            }
        }*/

        public Color toYCbCr(Color color)
        {
            byte Y, Cb, Cr;

            byte R = color.R;
            byte G = color.G;
            byte B = color.B;

            Y = (byte)(0 + (0.299 * R) + (0.587 * G) + (0.114 * B));
            Cb = (byte)(128 - (0.168736 * R) - (0.331264 * G) + (0.5 * B));
            Cr = (byte)(128 + (0.5 * R) - (0.418688 * G) - (0.081312 * B));

            return Color.FromArgb(Y, Cb, Cr);
        }


        public Color toY(Color color)
        {
            byte Y;

            byte R = color.R;
            byte G = color.G;
            byte B = color.B;

            Y = (byte)(0 + (0.299 * R) + (0.587 * G) + (0.114 * B));

            return Color.FromArgb(Y, Y, Y);
        }

        public Color toCb(Color color)
        {
            byte Cb;

            byte R = color.R;
            byte G = color.G;
            byte B = color.B;

            Cb = (byte)(128 - (0.168736 * R) - (0.331264 * G) + (0.5 * B));

            return Color.FromArgb(Cb, Cb, Cb);
        }


        public Color toCr(Color color)
        {
            byte Cr;

            byte R = color.R;
            byte G = color.G;
            byte B = color.B;

            Cr = (byte)(128 + (0.5 * R) - (0.418688 * G) - (0.081312 * B));

            return Color.FromArgb(Cr, Cr, Cr);
        }


        public unsafe void getYCbCr(Bitmap bmp, ref byte[,] Y_Data, ref byte[,] Cb_Data, ref byte[,] Cr_Data)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            Y_Data = new byte[width, height];            //luma 
            Cb_Data = new byte[width, height];           //Cb   - blue
            Cr_Data = new byte[width, height];           //Cr   - red

            unsafe
            {
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                int heightInPixels = bitmapData.Height;
                int widthInBytes = width * 3;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                //Convert to YCbCr
                for (int y = 0; y < heightInPixels; y++)
                {
                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < width; x++)
                    {
                        int xPor3 = x * 3;
                        /*float blue = currentLine[xPor3++];
                        float green = currentLine[xPor3++];
                        float red = currentLine[xPor3];*/
                        float red = currentLine[xPor3++];
                        float green = currentLine[xPor3++];
                        float blue = currentLine[xPor3];

                        Y_Data[x, y] = (byte)((0.299 * red) + (0.587 * green) + (0.114 * blue));
                        Cb_Data[x, y] = (byte)(128 - (0.168736 * red) - (0.331264 * green) + (0.5 * blue));
                        Cr_Data[x, y] = (byte)(128 + (0.5 * red) - (0.418688 * green) - (0.081312 * blue));
                    }
                }
                bmp.UnlockBits(bitmapData);
            }
        }



        public Bitmap reverseSubsample(double[,] Y, double[,] subCb, double[,] subCr)
        {
            Bitmap result = new Bitmap(Y.GetLength(0), Y.GetLength(1));

            int subRIndex = 0, subCIndex = 0;

            for (int row = 0; row < Y.GetLength(0); row++)
            {

                if (subRIndex >= subCb.GetLength(0)) break;
                subCIndex = 0;

                for (int col = 0; col < Y.GetLength(1); col++)
                {
                    if (subCIndex >= subCb.GetLength(1)) break;

                    result.SetPixel(row, col, Color.FromArgb((int)Y[row, col], (int)subCb[subRIndex, subCIndex],
                        (int)subCr[subRIndex, subCIndex]));
                    if (col % 2 != 0)
                    {
                        subCIndex++;
                    }
                }

                if (row % 2 != 0)
                {
                    subRIndex++;
                }
            }

            return result;
        }


        public Bitmap reverseSubsample(int[,] Y, int[,] subCb, int[,] subCr)
        {
            Bitmap result = new Bitmap(Y.GetLength(0), Y.GetLength(1));

            int subRIndex = 0, subCIndex = 0;
            int val1, val2, val3;

            for (int row = 0; row < Y.GetLength(0); row++)
            {

                if (subRIndex >= subCb.GetLength(0)) break;
                subCIndex = 0;

                for (int col = 0; col < Y.GetLength(1); col++)
                {
                    if (subCIndex >= subCb.GetLength(1)) break;
                    try
                    {
                        val1 = Y[row, col];
                        val2 = subCb[subRIndex, subCIndex];
                        val3 = subCr[subRIndex, subCIndex];//72 120

                        if (val3 > 255) val3 = 255;
                        if (val2 > 255) val2 = 255;
                        if (val1 > 255) val1 = 255;
                        if (val1 < 0) val1 = 0;
                        if (val2 < 0) val2 = 0;
                        if (val3 < 0) val3 = 0;

                        result.SetPixel(row, col, Color.FromArgb(val1, val2, val3));
                    }
                    catch (Exception e)
                    {
                        string gggg = e.Message;
                    }

                    if (col % 2 != 0)
                    {
                        subCIndex++;
                    }
                }

                if (row % 2 != 0)
                {
                    subRIndex++;
                }
            }

            return result;
        }


        //compress an image
        private void btnCompressImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileOpener = new OpenFileDialog(); //file selector
            fileOpener.Title = "Load Image";
            DialogResult result = fileOpener.ShowDialog();

            if (result == DialogResult.OK) // Test result.
            {
                selectedFilePath = fileOpener.FileName;
                selectedFileName = fileOpener.SafeFileName;
                loadImage(selectedFilePath);
                //infoLabel.Text = "File name: " + fileOpener.SafeFileName;
                infoLabel.Text = "Initial -> YCbCr -> revese subsampled back to rgb";
            }
        }



        //Allows user to choose a file from OpenFileDialog Gui
        private void btnFileSelect_Click(object sender, EventArgs e)
        {
            loadMotionVectors(); 
        }


        //MOTION VECTORS
        //
        //
        //
        // MOTION VECTOR P FRAME I FRAME
        ///////////////////

        //8x8 macro blocks
        int IMG_WIDTH, IMG_HEIGHT;


        public void loadMotionVectors()
        {
            try
            {
                OpenFileDialog fileOpener = new OpenFileDialog(); //file selector
                fileOpener.Title = "Load Image";
                DialogResult result = fileOpener.ShowDialog();
                if (result == DialogResult.OK) // Test result.
                {
                    selectedFilePath = fileOpener.FileName;
                    selectedFileName = fileOpener.SafeFileName;
                }

                Image img = Image.FromFile(selectedFilePath);
                IMG_WIDTH = img.Width; IMG_HEIGHT = img.Height;

                fileOpener = new OpenFileDialog(); //file selector
                fileOpener.Title = "Load Image 2";
                result = fileOpener.ShowDialog();
                if (result == DialogResult.OK) // Test result.
                {
                    selectedFilePath = fileOpener.FileName;
                    selectedFileName = fileOpener.SafeFileName;
                }

                Image img2 = Image.FromFile(selectedFilePath);


                imgDisplay.Image = img; //sets initial image
                Bitmap referenceBitmap = new Bitmap(img);
                Bitmap targetBitmap = new Bitmap(img2);

                DrawLineInt(targetBitmap, 35, 57, 36, 58);
                imgIntermediate.Image = targetBitmap;


                double[,] Y = null;
                double[,] Cb = null;
                double[,] Cr = null;

                double[,] Y_target = null;
                double[,] Cb_target = null;
                double[,] Cr_target = null;

                setYCbCr(referenceBitmap, ref Y, ref Cb, ref Cr);
                setYCbCr(targetBitmap, ref Y_target, ref Cb_target, ref Cr_target);


                List<int[,]> targetBlocks = new List<int[,]>();
                List<int[,]> referenceBlocks = new List<int[,]>();

                splitIntoMacroBlocks(Y, referenceBlocks);
                splitIntoMacroBlocks(Y_target, targetBlocks);

                //searchForMotionVector(referenceBlocks, targetBlocks, Y);


                //38 x 38
                //int rnum = getRow(89999, 300);          
                //int cnum = getColumn(89999, 300);


            }
            catch (Exception e)
            {
                if (e.Message != null)  MessageBox.Show("An exception has occured: " + e.Message);
            }
        }

        public void DrawLineInt(Bitmap bmp, int startX, int startY, int endX, int endY)
        {
            Pen blackPen = new Pen(Color.Red, 3);
            // Draw line to screen.
            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.DrawLine(blackPen, startX, startY, endX, endY);
            }
        }

        public int getRow(int blockNumber, int columnCount)
        {
            return blockNumber / columnCount;
        }

        public int getColumn(int blockNumber, int columnCount)
        {
            return blockNumber % columnCount;
        }


        public void splitIntoMacroBlocks(double[,] samples, List<int[,]> output)
        {

            int rowIndex, colIndex;
            int[,] result;
            int blocksHorizontal = (int)Math.Ceiling((double)IMG_WIDTH / 8);
            int blocksVertical = (int)Math.Ceiling((double)IMG_HEIGHT / 8);

            for (int r = 0; r < samples.GetLength(0); r += 8) //rows
            {
                for (int c = 0; c < samples.GetLength(1); c += 8) //columns
                {
                    result = new int[8, 8];

                    for (rowIndex = 0; rowIndex < 8; rowIndex++)
                    {
                        for (colIndex = 0; colIndex < 8; colIndex++)
                        {

                            if (colIndex + c >= samples.GetLength(1))
                            {
                                result[rowIndex, colIndex] = 0;

                            }
                            else if (rowIndex + r >= samples.GetLength(0))
                            {
                                result[rowIndex, colIndex] = 0;
                            }
                            else
                            {
                                result[rowIndex, colIndex] = (int)samples[rowIndex + r, colIndex + c];
                            }

                        }
                    }

                   output.Add(result);
                }
            }
        }



        public void searchForMotionVector(List<int[,]> referenceFrame, List<int[,]> targetFrame, double[,] refFrameData ,int searchArea=15)
        {
            if (referenceFrame.Count != targetFrame.Count) { MessageBox.Show("reference and target frames must be the same size!"); return; };

            int difference;

            for(int i = 0; i < referenceFrame.Count; i++)
            {
                difference = 0;

                for(int r = 0; r < 8; r++)
                {
                    for(int c = 0; c < 8; c++)
                    {
                        difference += Math.Abs(targetFrame[i][r,c] - referenceFrame[i][r, c]);
                    }
                }

                difference /= 64; //N^2, where N is the size of the macroblock, in this case 8

                if (difference > 0)
                {
                    MV motionVect; motionVect.difference = 99999999;

                    //int blocksHorizontal = (int)Math.Ceiling((double)IMG_WIDTH / 8);
                    //int blocksVertical = (int)Math.Ceiling((double)IMG_HEIGHT / 8);

                    int rnum = getRow(i*64, refFrameData.GetLength(1));
                    int cnum = getColumn(i * 64, refFrameData.GetLength(1));

                    int initialR = rnum - searchArea;
                    int initialC = cnum - searchArea;

                    for (; initialR < rnum + searchArea; initialR++)
                    {
                        for (;initialC < cnum + searchArea; initialC++)
                        {
                            MV tempMV = searchForClosestMatch(initialR, initialC, targetFrame[i], refFrameData);
                            if (tempMV.difference < motionVect.difference)
                            {
                                motionVect = tempMV;
                            }
                         }
                    }

                }

            }


        }

        public struct MV
        {
            public int row, col;
            public int difference;
        }

        public MV searchForClosestMatch(int rowCounter, int colCounter, int[,] targetFrame, double[,] refFrameData)
        {
            int difference = 0, x=0, y=0;
            

            for (int r = rowCounter; r < rowCounter+8; r++)
            {
                y = 0;
                for (int c = colCounter; c < colCounter+8; c++)
                {
                    if ( r >= refFrameData.GetLength(0) || r<0 || c >= refFrameData.GetLength(1) || c < 0) continue;

                    difference += Math.Abs(targetFrame[x, y++] - (int)(refFrameData[r, c]));
                }
                x++;
            }

            MV mVector;
            mVector.row = rowCounter;
            mVector.col = colCounter;
            mVector.difference = difference;

            return mVector; //macroblock 8x8
        }




        //end of class
    }
}

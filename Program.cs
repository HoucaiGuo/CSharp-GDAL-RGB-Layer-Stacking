using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSGeo.GDAL;
using OSGeo.OSR;

namespace RGBLayerStacking
{
    class Program
    {
        static void Main(string[] args)
        {
            //get the paths of rgb bands and output image
            Console.Write("Please enter the file path of blue band: ");
            string sB = Console.ReadLine();

            Console.Write("Please enter the file path of green band: ");
            string sG = Console.ReadLine();

            Console.Write("Please enter the file path of red band: ");
            string sR = Console.ReadLine();

            Console.Write("Please enter the file path of output tif image: ");
            string sOut = Console.ReadLine();

            //process
            RGBLayerStacking(sB, sG, sR, sOut);
        }

        
        static void RGBLayerStacking(string sBlueBand, string sGreenBand, string sRedBand, string sOut_Path)
        {
            //register gdal and set to support chinese path
            Gdal.AllRegister();
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");

            Dataset dsB = Gdal.Open(sBlueBand, Access.GA_ReadOnly);
            Dataset dsG = Gdal.Open(sGreenBand, Access.GA_ReadOnly);
            Dataset dsR = Gdal.Open(sRedBand, Access.GA_ReadOnly);

            Band bB = dsB.GetRasterBand(1);
            Band bG = dsG.GetRasterBand(1);
            Band bR = dsR.GetRasterBand(1);

            //get xy size and projection
            int iXSize = bB.XSize;
            int iYSize = bB.YSize;
            string sProjection = dsB.GetProjection();

            //create output image and set its projection
            Driver driverOut = Gdal.GetDriverByName("GTiff");
            Dataset dsOut = driverOut.Create(sOut_Path, iXSize, iYSize, 3, bB.DataType, null);
            dsOut.SetProjection(sProjection);

            //create 3 one dimension arrays to save data of each band
            double[] B = new double[iXSize * iYSize];
            double[] G = new double[iXSize * iYSize];
            double[] R = new double[iXSize * iYSize];

            //read the data of red band and write it to band 1 of output image
            bB.ReadRaster(0, 0, iXSize, iYSize, B, iXSize, iYSize, 0, 0);
            dsB.Dispose();
            bB.Dispose();
            dsOut.GetRasterBand(3).WriteRaster(0, 0, iXSize, iYSize, B, iXSize, iYSize, 0, 0);
            
            bG.ReadRaster(0, 0, iXSize, iYSize, G, iXSize, iYSize, 0, 0);
            dsG.Dispose();
            bG.Dispose();
            dsOut.GetRasterBand(2).WriteRaster(0, 0, iXSize, iYSize, G, iXSize, iYSize, 0, 0);

            bR.ReadRaster(0, 0, iXSize, iYSize, R, iXSize, iYSize, 0, 0);
            dsR.Dispose();
            bR.Dispose();
            dsOut.GetRasterBand(1).WriteRaster(0, 0, iXSize, iYSize, R, iXSize, iYSize, 0, 0);

            //release the memory
            dsOut.Dispose();
        }

    }
}

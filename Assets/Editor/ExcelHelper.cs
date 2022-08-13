using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using OfficeOpenXml;
using UnityEngine;

public class ExcelHelper
{
    [MenuItem("Excel/Load Excel")]
    static void LoadExcel()
    {
        string path = Application.dataPath + "/StreamingAssets/Excel/test.xlsx";
        //��ȡexcel
        using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using(ExcelPackage excel = new ExcelPackage(fs))
            {
                ExcelWorksheets workSheets = excel.Workbook.Worksheets;
                for (int i = 1; i < workSheets.Count; i++)
                {
                    ExcelWorksheet workSheet = workSheets[i];
                    int colCount = workSheet.Dimension.End.Column;
                    //��ȡ��ǰ����������
                    Debug.LogFormat("Sheet {0}", workSheet.Name);
                    for (int row = 1 , count = workSheet.Dimension.End.Row; row <= count;  row++)
                    {
                        for (int col = 1; col <= colCount; col++)
                        {
                            //��ȡÿ����Ԫ����
                            var text = workSheet.Cells[row, col].Text ?? "";
                            Debug.LogFormat("�±�:{0},{1} ���ݣ�{2}",row,col,text);
                        }
                    }
                }
            }
        }
    }
}

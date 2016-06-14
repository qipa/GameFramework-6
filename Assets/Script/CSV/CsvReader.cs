using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
public static class CSVReader
{
    public static string ReadCsvDic<TKEY, TRECORD>(string fileName, Dictionary<TKEY, TRECORD> dictRecord, string keyName, bool fieldNumMatch = false) where TRECORD : new()
    {
        Log.Info("正在加载配置表 : " + fileName);
        string ret = "";
        try
        {
            TextAsset CSVAsset = Resources.Load(fileName, typeof(TextAsset)) as TextAsset;

            byte[] Bytes = Encoding.UTF8.GetBytes(CSVAsset.text);
            MemoryStream ByteStream = new MemoryStream(Bytes);
            
            StreamReader reader = new StreamReader(ByteStream);

            string desc = reader.ReadLine();    //跳过第1行   第1行是说明
            string header = reader.ReadLine();  //第2行是字段名
            
            string skip = reader.ReadLine();    //跳过第3行    
            
            char[] sp = { ',' };
            string[] fields = header.Split(sp, System.StringSplitOptions.RemoveEmptyEntries);
            if (0 == fields.Length)
            {
                ret = fileName + ".csv  没有字段";
                Log.Error(ret);
                reader.Close();
                return ret;
            }

            System.Type tp_record = typeof(TRECORD);
            int fieldCount = tp_record.GetFields().Length;
            FieldInfo fieldKey = tp_record.GetField(keyName);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] records = line.Split(sp);
                if (records.Length != fields.Length)
                {
                    if (0 != records.Length)
                    {
                        Log.Info("data format error , line := " + line);
                    }
                    if (fieldNumMatch && (records.Length < fields.Length / 2))
                    {
                        continue;  //csv 进行版本兼容,不强制字段数量匹配
                    }
                }

                try
                {
                    TRECORD record = new TRECORD();
                    uint ok_count = 0;
                    for (int i = 0; i < fields.Length; ++i)
                    {
                        if (i >= records.Length)
                            break;
                        FieldInfo info = tp_record.GetField(fields[i]);
                        if (SetField(record, info, records[i]))
                        {
                            ok_count += 1;
                        }
                        else
                        {
                            if (fields[i] == keyName)
                            {
                                ok_count = 0;
                                break;
                            }
                        }
                    }
                    if (0 == ok_count)
                    {
                        Debug.LogError("   ok_count == 0");
                        continue;
                    }
                    if (fieldNumMatch && (ok_count < fieldCount * 0.75f))  // 数量匹配有误
                    {
                        continue;
                    }
                    dictRecord.Add((TKEY)fieldKey.GetValue(record), record);
                }
                catch(System.Exception e)
                {
                    Debug.LogError(e.Message);
                }

            }
            reader.Close();
            return ret;
        }
        catch (System.Exception ex)
        {
            ret = "exception := " + ex.Message;
        }

        return ret;
    }

    public static string ReadCsvList<TRECORD>(string fileName, List<TRECORD> listRecord, bool fieldNumMatch = false) where TRECORD : new()
    {
        string ret = "";
        try
        {
            TextAsset CSVAsset = Resources.Load(fileName, typeof(TextAsset)) as TextAsset;
           

            byte[] Bytes = Encoding.UTF8.GetBytes(CSVAsset.text);
            MemoryStream ByteStream = new MemoryStream(Bytes);
            StreamReader reader = new StreamReader(ByteStream);

            string desc = reader.ReadLine();    //跳过第1行   第1行是说明
            string header = reader.ReadLine();  //第2行是字段名

            string skip = reader.ReadLine();    //跳过第3行  

            char[] sp = { ',' };
            string[] fields = header.Split(sp, System.StringSplitOptions.RemoveEmptyEntries);
            if (0 == fields.Length)
            {
                ret = fileName + ".csv  没有字段";
                reader.Close();
                return ret;
            }
            System.Type tp_record = typeof(TRECORD);
            int fieldCount = tp_record.GetFields().Length;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] records = line.Split(sp);
                if (records.Length != fields.Length)
                {
                    if (0 != records.Length)
                    {
                        Log.Info("data format error , line := " + line);
                    }
                    if (fieldNumMatch && (records.Length < fields.Length / 2))
                    {
                        continue;  //csv 进行版本兼容,不强制字段数量匹配
                    }
                }
                if (records[0].Contains("//"))
                {
                    continue;
                }
                try
                {
                    TRECORD record = new TRECORD();
                    uint ok_count = 0;
                    for (int i = 0; i < fields.Length; ++i)
                    {
                        if (i >= records.Length) break;
                        if (SetField(record, tp_record.GetField(fields[i]), records[i]))
                        {
                            ok_count += 1;
                        }
                    }
                    if (fieldNumMatch && (ok_count < fieldCount * 0.75f))  // 数量匹配有误
                    {
                        continue;
                    }
                    listRecord.Add(record);
                }
                catch (System.Exception ex)
                {
                    Log.Exception(ex);
                }
            }
            reader.Close();
            return ret;
        }
        catch (System.Exception ex)
        {
            ret = "exception := " + ex.Message;
        }
        return ret;
    }

    public static bool SetField(object obj, FieldInfo fi, string data)
    {
        if (null == fi || null == data)
        {
            return false;
        }
        if ("" == data)
        {
            return true; // 默认值可不填
        }
        bool ret = false;

        if (fi.FieldType == typeof(int))
        {
            int val = 0;
            ret = int.TryParse(data, out val);
            fi.SetValue(obj, val);
        }
        else if (fi.FieldType == typeof(uint))
        {
            uint val = 0;
            ret = uint.TryParse(data, out val);
            fi.SetValue(obj, val);
        }
        else if (fi.FieldType == typeof(string))
        {
            fi.SetValue(obj, data);
            ret = true;
        }
        else if (fi.FieldType == typeof(float))
        {
            float val = 0;
            ret = float.TryParse(data, out val);
            fi.SetValue(obj, val);
        }
        else if (fi.FieldType == typeof(bool))
        {
            bool val = false;
            if (!bool.TryParse(data, out val))
            {
                int val2 = 0;
                ret = int.TryParse(data, out val2);
                val = (0 != val2);
                fi.SetValue(obj, val);
            }
            else
            {
                ret = true;
                fi.SetValue(obj, val);
            }
        }
        else if (fi.FieldType == typeof(short))
        {
            short val = 0;
            ret = short.TryParse(data, out val);
            fi.SetValue(obj, val);
        }
        else if (fi.FieldType == typeof(ushort))
        {
            ushort val = 0;
            ret = ushort.TryParse(data, out val);
            fi.SetValue(obj, val);
        }
        else if (fi.FieldType == typeof(double))
        {
            double val = 0;
            ret = double.TryParse(data, out val);
            fi.SetValue(obj, val);
        }
        else if (fi.FieldType == typeof(byte))
        {
            byte val = 0;
            ret = byte.TryParse(data, out val);
            fi.SetValue(obj, val);
        }
        else if (fi.FieldType == typeof(sbyte))
        {
            sbyte val = 0;
            ret = sbyte.TryParse(data, out val);
            fi.SetValue(obj, val);
        }
        return ret;
    }

    public static string GetField(object obj, FieldInfo fi)
    {
        if (null == fi)
        {
            return "";
        }

        object val = fi.GetValue(obj);
        if (System.Convert.ChangeType(val, typeof(string)) != null)
        {
            return val.ToString();
        }
        return "";
    }
}


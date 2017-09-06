using System;
using System.IO;

//提供unity不支持的通讯需要方法
class NetUitl
{
    //复制流
    public static void CopyStream(Stream src, Stream des)
    {
        int bufferSize = 4096;
        byte[] buffer = new byte[bufferSize];
        while (true)
        {
            int read = src.Read(buffer, 0, buffer.Length);
            if (read <= 0)
            {
                return;
            }
            des.Write(buffer, 0, read);
        }
    }

    //复制流
    public static void CopyStream(Stream src, Stream des, int length)
    {
        byte[] buffer = new byte[length];
        int read = src.Read(buffer, 0, length);
        if (read <= 0)
        {
            return;
        }
        des.Write(buffer, 0, read);
    }

}

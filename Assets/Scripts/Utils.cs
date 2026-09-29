using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System;
using UnityEngine.Networking;
using System.Net;
using System.Globalization;
using System.Net.Sockets;

public static class Utils {

    public static float GetRandomFloatBetween(float a, float b)
    {
        return (float)UnityEngine.Random.Range(a*10, b * 10) / 10;
    }
    public static bool IsValidEmailAddress(string s)
    {
        var regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
        return regex.IsMatch(s);
    }
    public static void RemoveAllChildsIn(Transform container)
     {
         int num = container.transform.childCount;
         for (int i = 0; i < num; i++) UnityEngine.Object.DestroyImmediate(container.transform.GetChild(0).gameObject);
     }
     public static void Shuffle(List<int> texts)
     {
         if (texts.Count < 2) return;
         for (int a = 0; a < 100; a++)
         {
             int id = UnityEngine.Random.Range(1, texts.Count);
             int value1 = texts[0];
            int value2 = texts[id];
             texts[0] = value2;
             texts[id] = value1;
         }
     }
    public static bool FloatIsNearOtherFloat(float a, float b, float diff)
    {
        return (Mathf.Abs(a - b) < diff);
    }
    public static void Shuffle(AudioClip[] arr)
    {
        if (arr.Length < 2) return;
        for (int a = 0; a < 100; a++)
        {
            int id = UnityEngine.Random.Range(1, arr.Length);
            AudioClip value1 = arr[0];
            AudioClip value2 = arr[id];
            arr[0] = value2;
            arr[id] = value1;
        }
    }
    public static class CoroutineUtil
	{
		public static IEnumerator WaitForRealSeconds(float time)
		{
			float start = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup < start + time)
			{
				yield return null;
			}
		}
	}
	public static string FormatNumbers(int num, bool toLetters = false)
	{
        if(toLetters)
            return ToFormattedString(num);
        else
            return num.ToString("#,#", CultureInfo.InvariantCulture); // fuerza siempre comas:
        // return string.Format ("{0:#,#}",  num);                  // devuelve coma o punto segun el idioma:
    }
    public static List<FileInfo> GetFilesInFolder(string url)
    {
        List<FileInfo> arr = new List<FileInfo>();
        DirectoryInfo dir = new DirectoryInfo(url);
        FileInfo[] info = dir.GetFiles("*.*");
        foreach (FileInfo f in info)
        {
            if(!f.Name.Contains(".meta"))
                arr.Add(f);
        }
        return arr;
    }
    public static string ToFormattedString(this double rawNumber)
    {
        string[] letters = new string[] { "", "K", "M", "B", "T", "P", "E", "Z", "Y", "KY", "MY", "BY", "TY", "PY", "EY", "ZY", "YY" };
        int prefixIndex = 0;
        while (rawNumber > 1000)
        {
            rawNumber /= 1000.0f;
            prefixIndex++;
            if (prefixIndex == letters.Length - 1)
            {
                break;
            }
        }
        string numberString = rawNumber.ToString();
        if (prefixIndex < letters.Length - 1)
        {
            numberString = ToThreeDigits(numberString);
        }

        string prefix = letters[prefixIndex];
        return $"{numberString}{prefix}";
    }
    private static string ToThreeDigits(string numString)
    {
        if (numString.Length > 4)
        {
            if (numString.Substring(0, 4).Contains("."))
                numString = numString.Substring(0, 5);
            else
                numString = numString.Substring(0, 4);
        }
        return numString;
    }
    public static string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
    public static string SHA(string strToEncrypt)
    {
        using (SHA256 mySHA256 = SHA256.Create())
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            byte[] hashBytes = mySHA256.ComputeHash(bytes);
            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');

            return hashString.PadLeft(32, '0');
        }
    }
    public static void Shuffle<T>(List<T> list)
    {
        System.Random _random = new System.Random();
        int n = list.Count;
        for (int i = 0; i < n; i++)
        {
            // Use Next on random instance with an argument.
            // ... The argument is an exclusive bound.
            //     So we will not go past the end of the array.
            int r = i + _random.Next(n - i);
            T t = list[r];
            list[r] = list[i];
            list[i] = t;
        }
    }

    public static void Shuffle<T>(T[] array)
    {
        System.Random _random = new System.Random();
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            // Use Next on random instance with an argument.
            // ... The argument is an exclusive bound.
            //     So we will not go past the end of the array.
            int r = i + _random.Next(n - i);
            T t = array[r];
            array[r] = array[i];
            array[i] = t;
        }
    }

    public static string Today(bool serverTime = true) {
        DateTime now = DateTime.UtcNow;
        if (serverTime)
            now = NetworkTime();
        return now.Year + "" + now.Month + "" + now.Day;
        //return System.DateTime.UtcNow.Year + "" + System.DateTime.UtcNow.Month + "" + System.DateTime.UtcNow.Day;
    }

    public static string GetDayTimeCountdown() {
        return GetDayTimeCountdown(DateTime.UtcNow);
    }

    public static string GetDayTimeCountdown(DateTime now) {
        int h1 = now.Hour;
        int m1 = now.Minute;
        int s1 = now.Second;

        int hours = (int)(24 - h1);
        int mins = 60 - m1;
        int sec = 60 - s1;

        int h = (int)(hours - 1);
        if (h < 0) h = 0;

        string hoursString = "";

        if (h < 9)
            hoursString = "0";

        hoursString += h.ToString();
        hoursString += ":";

        if (mins < 9) hoursString += "0";

        hoursString += mins;
        hoursString += ":";

        if (sec < 9) hoursString += "0";

        hoursString += sec;

        return hoursString;
    }

    public static string GetNextHourCountdown() {
        return GetNextHourCountdown(DateTime.UtcNow);
    }

    public static string GetNextHourCountdown(DateTime now) {
        int m1 = now.Minute;
        int s1 = now.Second;

        int mins = 59 - m1;
        int sec = 59 - s1;

        string minutesString = "";

        if (mins <= 9) minutesString += "0";

        minutesString += mins;
        minutesString += ":";

        if (sec <= 9) minutesString += "0";

        minutesString += sec;        

        return minutesString;
    }

    public static float GetNextHourProgress() {
        return GetNextHourProgress(DateTime.UtcNow);
    }

    public static float GetNextHourProgress(DateTime now) {

        int sec = 60 * (59 - now.Minute) + 59 - now.Second;

        return 1f*sec / 3600;
    }

    public static void PrintColor(string color, object text, UnityEngine.Object cont = null)
    {
        if (cont != null)
            Debug.Log("<color=" + color + ">" + text + "</color>", context: cont);
        else
            Debug.Log("<color=" + color + ">" + text + "</color>");
    }
    private static object WWW(string url)
    {
        throw new NotImplementedException();
    }

    public static System.DateTime NetworkTime(int utc=0, int index = 0) {
        //default Windows time server
        string[] ntpServer = { "time.windows.com", "time.nist.gov", "time-nw.nist.gov", "time-a.nist.gov", "time-b.nist.gov" };
        //const string ntpServer = "time.nist.gov";

        // NTP message size - 16 bytes of the digest (RFC 2030)
        var ntpData = new byte[48];

        //Setting the Leap Indicator, Version Number and Mode values
        ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

        var addresses = Dns.GetHostEntry(ntpServer[index]).AddressList;

        //The UDP port number assigned to NTP is 123
        var ipEndPoint = new IPEndPoint(addresses[0], 123);
        //NTP uses UDP
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        try {
            socket.Connect(ipEndPoint);

            //Stops code hang if NTP is blocked
            socket.ReceiveTimeout = 3000;

            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = System.BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = System.BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            double milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            milliseconds += utc * 60 * (60 * 1000);
            //**UTC** time
            var networkDateTime = (new System.DateTime(1900, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            //return networkDateTime.ToLocalTime();
            return networkDateTime;
        }
        // Manage of Socket's Exceptions
        catch (ArgumentNullException ane) {
            Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
        } catch (SocketException se) {
            Console.WriteLine("SocketException : {0}", se.ToString());
        } catch (Exception e) {
            Console.WriteLine("Unexpected exception : {0}", e.ToString());
        }

        if (index < ntpServer.Length-1)
            return NetworkTime(utc, index + 1);
        else {
            if(utc==0)
                return DateTime.UtcNow;
            else
                return DateTime.Now;
        }
    }

    // stackoverflow.com/a/3294698/162671
    static uint SwapEndianness(ulong x) {
        return (uint)(((x & 0x000000ff) << 24) +
                       ((x & 0x0000ff00) << 8) +
                       ((x & 0x00ff0000) >> 8) +
                       ((x & 0xff000000) >> 24));
    }
}
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;




public struct WebsiteData
{
    public string WebSite { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public WebsiteData(/*string allResoults*/string webSite, string userName, string password)
    {
        //string[] items = allResoults.Split("; ");
        WebSite = webSite;
        UserName = userName;
        Password = password;
    }

    public override string ToString()
    {
        return WebSite + "; " + UserName + "; " + Password;
    }
}




public class CDictionary<T, WebsiteData> : Dictionary<T, WebsiteData>
{
    public override string ToString()
    {
        string retVal = string.Empty;

        foreach (KeyValuePair<T, WebsiteData> item in this)
        {
            retVal += item.Value.ToString() + "\n";
        }
        return retVal;
    }
}




interface IDataWriter
{
    public void StoreAccounts(CDictionary<string, WebsiteData> acounts);
}




public class DataWriter : IDataWriter
{
    string FilePath;

    public DataWriter(string path)
    {
        FilePath = path;
    }

    public void StoreAccounts(CDictionary<string, WebsiteData> acounts)
    {
        File.WriteAllText(FilePath, acounts.ToString());
    }
}

public class DataWriterJson : IDataWriter
{
    string FilePath;
    string json;

    public DataWriterJson(string path)
    {
        FilePath = path;
    }

    public void StoreAccounts(CDictionary<string, WebsiteData> acounts)
    {
        json = JsonSerializer.Serialize(acounts);
        File.WriteAllText(FilePath, json);
    }
}



interface IDataReader
{
    public CDictionary<string, WebsiteData> GetAllAccounts();
}




public class DataReader : IDataReader
{
    string FilePath;
    public DataReader(string filepath)
    {
        FilePath = filepath;
    }

    public CDictionary<string, WebsiteData> GetAllAccounts()
    {
        IEnumerable<string> lines = File.ReadLines(FilePath);

        CDictionary<string, WebsiteData> accounts = new CDictionary<string, WebsiteData>();

        List<string> accountsI = new();
        foreach (var line in lines)
        {
            accountsI.Add(line);
        }

        for (int i = 0; i < accountsI.Count; i++)
        {
            string[] items = accountsI[i].Split("; ");
            WebsiteData site = new WebsiteData(items[1], items[2], items[3]);
            accounts.Add(site.WebSite, site);
        }

        return accounts;
    }
}

public class DataReaderJson : IDataReader
{
    string FilePath;
    public DataReaderJson(string filepath)
    {
        FilePath = filepath;
    }

    public CDictionary<string, WebsiteData> GetAllAccounts()
    {
        string json = File.ReadAllText(FilePath);
        CDictionary<string, WebsiteData> accounts = JsonSerializer.Deserialize<CDictionary<string, WebsiteData>>(json);

        return accounts;
    }
}




public class AccountsOperations
{

    string FilePath;
    public CDictionary<string, WebsiteData> allAccounts = new CDictionary<string, WebsiteData>();

    public AccountsOperations(string filepath)
    {
        FilePath = filepath;

        DataReaderJson dataReader = new DataReaderJson(FilePath);
        allAccounts = dataReader.GetAllAccounts();
    }

    public bool AddAcount(WebsiteData account)
    {
        return allAccounts.TryAdd(account.WebSite, account);
    }

    public bool DeleteAcount(string webSite)
    {
        return allAccounts.Remove(webSite);
    }

    public bool EditAcount(WebsiteData account)
    {
        if (!allAccounts.ContainsKey(account.WebSite))
        {
            return false;
        }
        allAccounts[account.WebSite] = account;
        return true;
    }

    public CDictionary<string, WebsiteData> GetAllAccounts(){
        return allAccounts;
    }

    public void SavedData()
    {
        DataWriterJson savedData = new DataWriterJson(FilePath);

        savedData.StoreAccounts(allAccounts);
    }
}




static class PaswordProj
{
    static int Main(string[] args)
    {
        string datafilparth = "C:\\Users\\malya\\C#-program\\C#\\Prog1\\test.txt";
        string datafilparthjson = "C:\\Users\\malya\\C#-program\\C#\\Prog1\\JsonText.json";

        AccountsOperations allData = new AccountsOperations(datafilparthjson);

        string methodNum;

        Console.WriteLine("Hello!  Add;   Delet;  Edit;  GetAll;");
        methodNum = Console.ReadLine();

        switch(methodNum)
        {
            default: 
                Console.WriteLine("NO");
                break;

            case ("Add"):
                WebsiteData addedAccount = new WebsiteData();

                Console.WriteLine("Website:");
                addedAccount.WebSite = Console.ReadLine();
                Console.WriteLine("Username:");
                addedAccount.UserName = Console.ReadLine();
                Console.WriteLine("Password:");
                addedAccount.Password = Console.ReadLine();

                Console.WriteLine(allData.AddAcount(addedAccount));

                break;

            case ("Delet"):
                string webSite;
                webSite = Console.ReadLine();
                break;
            case ("Edit"):
                WebsiteData EditedAccount = new WebsiteData();

                Console.WriteLine("Website:");
                EditedAccount.WebSite = Console.ReadLine();
                Console.WriteLine("Username:");
                EditedAccount.UserName = Console.ReadLine();
                Console.WriteLine("Password:");
                EditedAccount.Password = Console.ReadLine();

                Console.WriteLine(allData.EditAcount(EditedAccount));

                break;

            case ("GetAll"):
                Console.WriteLine(allData.allAccounts.ToString());

                break;
        }

        allData.SavedData();

        return 0;
    }
}


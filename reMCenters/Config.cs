using System;
using System.IO;
using System.Xml.Serialization;
public enum ModOptions{ DllMethodAutoPatch,DllMethodOnline,DllMethodAutoPatchNonPermanent,reMCenters5,HookMethod}

public class Config
{
    private static readonly string ConfigFilePath = "C:\\ProgramData\\reMCenters\\config.xml";

    public ModOptions SelectedMod { get; set; }
    private static Config ConfigCache;

    public void Save()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Config));
        using (StreamWriter writer = new StreamWriter(ConfigFilePath))
        {
            serializer.Serialize(writer, this);
        }
    }

    public static Config Load()
    {
        if(ConfigCache != null) return ConfigCache;
        if (!File.Exists(ConfigFilePath))
        {
            ConfigCache=new Config();
            return ConfigCache;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(Config));
        using (StreamReader reader = new StreamReader(ConfigFilePath))
        {
            ConfigCache= (Config)serializer.Deserialize(reader);
            return ConfigCache;
        }
    }
}

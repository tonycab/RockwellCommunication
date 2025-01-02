using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RockwellCommunication.Protocol
{

    public class Member
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public int Dimension { get; set; } = 0;

        public bool Hidden { get; set; } = false;

        public Member(string name, string type, int dimension,bool hidden) {
            Name = name;
            Type = type;
            Dimension = dimension;
            Hidden = hidden;
        }


        public override string ToString() => $"{Name} - {Type}";
    }

    public class DataType
    {
        public string Name { get; set; }

        public bool Target { get; set; }

        public List<Member> Types { get; set; } = new List<Member>();

        public DataType(string name, bool target=false)
        {
            Name = name;
            Target = target;
        }

        public override string ToString() => Name;

    }

    public class UDT_Manager
    {  
        /// <summary>
        /// List of UDT
        /// </summary>
        public Dictionary<string,DataType> listUdt = new Dictionary<string,DataType>();

        public string UdtName { get; set; }

        private static UDT_Manager FromXml(XElement xElement)
        {
            UDT_Manager udt = new UDT_Manager();


            foreach (var item in xElement.Elements("Controller").Elements("DataTypes").Elements("DataType")) {
                
                DataType dataType;

                if ((item.Attribute("Use")?.Value) != null && (item.Attribute("Use").Value) == "Target")
                {
                    dataType = new DataType(item.Attribute("Name").Value,true);
                    udt.UdtName = item.Attribute("Name").Value;

                } else {
                     dataType = new DataType(item.Attribute("Name").Value);
                }

                var members = item.Element("Members");

                foreach (var member in members.Elements("Member")){

                    dataType.Types.Add(new Member(member.Attribute("Name").Value, member.Attribute("DataType").Value, int.Parse(member.Attribute("Dimension").Value), member.Attribute("Hidden").Value=="true"?true:false ));

                }
                    udt.listUdt.Add(dataType.Name, dataType);
             }

                return udt;
        }


        /// <summary>
        /// Return instance of UDT_Manager
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static UDT_Manager LoadToXml(string path)
        {

            XDocument xDocument = XDocument.Load(path);

            return FromXml(xDocument.Root);

        }


    }
}

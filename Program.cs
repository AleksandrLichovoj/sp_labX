using System.Xml.Linq;
using System.Linq;
using System.Globalization;
using System.ComponentModel;
// See https://aka.ms/new-console-template for more information

using Microsoft.VisualBasic.FileIO;
// 0 , 1 , 2, 3, 4, 5;
//id;adresas;namo_valdytojas;valdymo_forma;paskyrimo_pagrindas;administratoriaus_pabaigos_Data;
//paskirtis;uni_nr;bendr_plotas;naud_plotas;
//build_year;renov_metai;renovacijos_statusas;energ_naudingumo_klase;butu_skaicius;
//negyvenamuju_palapu_skaicius;korpusas;sklypo_plotas

//1;A. Goštauto g. 2; 286-oji gyvenamojo namo A.Goštauto g. 2/15 savininkų bendrija;Bendrija;1995.02.02;;Gyvenamoji (trys ir daugiau butų);1096-0010-4010;6607.43;4360.68;1960;;Nerenovuotas;;53;4;1A7p;0.00
//11;A. Paškevič-Ciotkos g. 15;843-oji daugiabučio namo savininkų bendrija;Bendrija;2002.02.11;;Gyvenamoji (trys ir daugiau butų);1094-0222-5032;1465.63;1176.41;1996;;Nerenovuotas;;19;0;3A5p;0.00

internal class Program
{
    private static void Main(string[] args)
    {
        var path = @"apartment_buildings_2019.csv";

        var buildingList = new Dictionary<string, Element>();

        int lowestYear = -1;

        int highestYear = 0;

        int compareYear = 0;


        // Habeeb, "Dubai Media City, Dubai"
        using (TextFieldParser csvParser = new TextFieldParser(path))
        {
            csvParser.CommentTokens = new string[] { "#" };
            csvParser.SetDelimiters(new string[] { ";" });
            csvParser.HasFieldsEnclosedInQuotes = true;

            // Skip the row with the column names
            csvParser.ReadLine();

            bool ignore = false;

            while (!csvParser.EndOfData)
            {
                // Read current line fields, pointer moves to the next line.

                string[] fields = csvParser.ReadFields();
                string id = fields[0];
                string address = fields[1];
                string owner = fields[2];
                string ownershipForm = fields[3];
                string adminstrationEndDate = fields[4];
                string appointmentBasis = fields[5];
                string purpose = fields[6];
                string uniNumber = fields[7];
                string generalSize = fields[8];
                string usedSize = fields[9];

                string buildYear = fields[10];
                string renovationYear = fields[11];
                string renovationStatus = fields[12];
                string energyClass = fields[13];
                string houseCount = fields[14];
                string emtyHouseCount = fields[15];
                string corpus = fields[16];
                string areaSize = fields[17];

                AddToDictionary(buildingList,id,address,owner,ownershipForm,adminstrationEndDate, 
                appointmentBasis,purpose,uniNumber,generalSize,usedSize,buildYear,renovationYear,
                renovationStatus,energyClass,houseCount,emtyHouseCount,corpus,areaSize);
                
                
                

                bool isParsable = Int32.TryParse(buildYear, out compareYear);
                if (!isParsable)
                    ignore = true;
                else
                {
                    if(compareYear == 0) ignore = true;
                }
                
                if(!ignore){


                    if (compareYear > highestYear) highestYear = compareYear;
                    else
                    {
                        if(lowestYear == -1){
                            lowestYear = compareYear;
                        }
                        else{
                            if(lowestYear > compareYear) lowestYear = compareYear;
                        }
                    }
                }
                Console.WriteLine(buildYear);
                //Console.WriteLine("LOWEST: " + lowestYear);
                //Console.WriteLine("HIGHEST: " + highestYear);
            }
            Console.WriteLine("LOWEST: " + lowestYear);
            Console.WriteLine("HIGHEST: " + highestYear);
            //Console.WriteLine(buildingList.Values.SelectMany(Element => Element.BuildYear).ToList());
            Dictionary<string, Element>.ValueCollection values =buildingList.Values;
            foreach (Element val in values)
            {  
            Console.WriteLine("Id", val.Id);  
            }
           

        }

       


    }

    public static void AddToDictionary(Dictionary<string, Element> elements,
            string id,
            string address,
            string owner,
            string ownershipForm,
            string adminstrationEndDate,
            string appointmentBasis,
            string purpose,
            string uniNumber,
            string generalSize,
            string usedSize,
            string buildYear,
            string renovationYear,
            string renovationStatus,
            string energyClass,
            string houseCount,
            string emtyHouseCount,
            string corpus,
            string areaSize)

        {
            Element theElement = new Element();

            theElement.Id = id;
            theElement.Address = address;
            theElement.Owner = owner;
            theElement.OwnershipForm = ownershipForm;
            theElement.AdminstrationEndDate = adminstrationEndDate;
            theElement.AppointmentBasis = appointmentBasis;
            theElement.UniNumber = uniNumber;
            theElement.GeneralSize = generalSize;
            theElement.UsedSize = usedSize;
            theElement.BuildYear = buildYear;
            theElement.RenovationYear = renovationYear;
            theElement.RenovationStatus = renovationStatus;
            theElement.EnergyClass = energyClass;
            theElement.HouseCount = houseCount;
            theElement.EmtyHouseCount = emtyHouseCount;
            theElement.Corpus = corpus;
            theElement.AreaSize = areaSize;



            elements.Add(key: theElement.Id, value: theElement);
        }

        public void ConstructBuckets(int lowestYear, int highestYear, int period, Dictionary<string, Element> houseList){
            int duration = highestYear - lowestYear;
            int bucketNumber = (int)Convert.ToSingle(duration/period); 

            foreach( KeyValuePair<string, Element> element in houseList){
                
            }

        }

}

public class Element 
{
    public string Id { get; set; }
    public string Address { get; set; }
    public string Owner { get; set; }
    public string OwnershipForm { get; set; }
    public string AdminstrationEndDate { get; set; }
    public string AppointmentBasis { get; set; }
    public string UniNumber { get; set; }
    public string GeneralSize { get; set; }
    public string UsedSize { get; set; }
    public string BuildYear { get; set; }
    public string RenovationYear { get; set; }

    public string RenovationStatus { get; set;}
    public string EnergyClass { get; set; }
    public string HouseCount { get; set; }
    public string EmtyHouseCount { get; set; }
    public string Corpus { get; set; }
    public string AreaSize { get; set; }
   
}





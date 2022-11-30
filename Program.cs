using System.Xml.Linq;
using System.Linq;
using System.Globalization;
using System.ComponentModel;


using Microsoft.VisualBasic.FileIO;


// Distribution of building years built by 5 year buckets

internal class Program
{
    // Our main function, where all function initialized
    private static void Main(string[] args)
    {
        var path = @"apartment_buildings_2019.csv";

        // We using two dictionary lists, one for 5 years buckets, other for building data.

        var buildingList = new Dictionary<string, Element>();

        var bucketList = new Dictionary<int, Bucket>();

        // Variables for computations

        int lowestYear = -1;

        int highestYear = 0;

        int compareYear = 0;

        // Here we define which duratation should bucket be, by our task it 5.

        int sort_by_year = 5;


        // Reading from CVS file function
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

                // Add readed data to dictionary 

                AddToDictionary(buildingList,id,address,owner,ownershipForm,adminstrationEndDate, 
                appointmentBasis,purpose,uniNumber,generalSize,usedSize,buildYear,renovationYear,
                renovationStatus,energyClass,houseCount,emtyHouseCount,corpus,areaSize);
                
                
                // We need to check  if build year is given in the file

                bool isParsable = Int32.TryParse(buildYear, out compareYear);
                if (!isParsable)
                    ignore = true;
                else
                {
                    if(compareYear == 0) ignore = true; // if not, we ignore this computation
                }

                // Here we find out the highest year and lowest year in the list
                
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
               
            }
            
            // We create our buckets sorted by 5 years
            
            bucketList = ConstructBuckets(lowestYear,highestYear,sort_by_year,buildingList);
           
            // Function for console writing our results
            DisplayInConsole(bucketList, lowestYear ,highestYear, buildingList );

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

        

        public static Dictionary<int,Bucket> ConstructBuckets(int lowestYear, int highestYear, int period, Dictionary<string, Element> list){
            
            
            
            var bucketList = new Dictionary<int, Bucket>();
            
            // Calculations to find out, how many sorted bucket we need
            
            int duration = highestYear - lowestYear;
            
            int bucketNumber = (int)Convert.ToSingle(duration/period);  // We may get fractions, convert to integer

            int bucket_startYear = 0;
            
            int bucket_endYear = 0;

            // We need to find starting bucket range, below our calculation of doing that
            // n is correcting value
            int n = 0;

            decimal condition = decimal.Divide(lowestYear-n,period);
            
            // here we try to find n, so that by sorting our lowest year, there would be no fractions.

            while(!decimal.IsInteger(condition)){
                n++; // 4
                condition = decimal.Divide(lowestYear-n,period);
                
            };

            // so in case we have 1904, it went to first bucket range 1900-1905
            // Calculateing first bucket range
            bucket_startYear = lowestYear - n;

          

            
            // Creating bucket dictionary with sorted ranges

            for(int i = 0; i <= bucketNumber; i++){
                Bucket bucket = new Bucket();
                bucket.start_range = bucket_startYear;
                
                bucket_endYear = bucket_startYear + period;
                bucket.end_range = bucket_startYear + period;
                
              
                bucket.buidingList_id = new List<string>();
                bucketList.Add(key:i, value: bucket);
                bucket_startYear = bucket_endYear;

            }

            int buildYear = 0;
            
            // Assigning bulding build years into buckets

            foreach (KeyValuePair<string, Element> building in list)
            {
                
                 foreach(KeyValuePair<int, Bucket> buckets in bucketList){
                    int.TryParse(building.Value.BuildYear, out buildYear);
                    if(buildYear >= buckets.Value.start_range && buildYear < buckets.Value.end_range){
                       buckets.Value.buidingList_id?.Add(building.Value.Id); 
                     
                    } 
                 }
            }

            return bucketList;

        }


        public static void DisplayInConsole(Dictionary<int,Bucket> bucketList, int lowestYear, int highestYear , Dictionary<string,Element> buildingList ){

            string bucketKey; 

            Console.WriteLine("---START WRITING---");
            Console.WriteLine(" ");
            Console.WriteLine("The lowest build year in the bucket : " + lowestYear);
            Console.WriteLine("The highest build year in the bucket : " + highestYear);

            foreach(KeyValuePair<int,Bucket> bucket in bucketList){
                
                Console.WriteLine("Building builds from " + bucket.Value.start_range + " to " + bucket.Value.end_range + " are " + bucket.Value.buidingList_id?.Count + " in the list , bucket number " + bucket.Key );
               
                
            }

            Console.WriteLine(" ");
            Console.WriteLine("Which bucket to display? :");
            bucketKey = Console.ReadLine();

            if(int.TryParse(bucketKey,out int key)){
                Console.WriteLine("This bucket building list ids: ");
                if(bucketList.ContainsKey(key)){
                    foreach(var i in bucketList[key].buidingList_id)
                    {
                         Console.WriteLine(i + " Build year: " + buildingList[i].BuildYear);
                        
                        
                    }
                }else
                {
                    Console.WriteLine("! Suck bucket id doesnt exist");
                }
                
            }
            else{
                Console.WriteLine("! Wrong input...");
            }

            

            
            Console.WriteLine("");
            Console.WriteLine("---END WRITING---");


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

public class Bucket{
    
    public int start_range{ get;set;}

    public int end_range{ get; set;}
    public List<string>? buidingList_id { get; set;}

}





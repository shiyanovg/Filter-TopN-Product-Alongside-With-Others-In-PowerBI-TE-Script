string ListOfTable = "" ;
string TN = "TopN";
string PN = "Product Names";

 foreach(var table in Model.Tables) {
     // table.Name.Output();
    ListOfTable = ListOfTable + (table.DaxObjectName ) + ",";     
 };

bool TopNCheck = ListOfTable.IndexOf(TN) >= 0;
bool ProductNamesCheck = ListOfTable.IndexOf(PN) >= 0;

if (!TopNCheck || !ProductNamesCheck) {
    // 
    Error("No 'TopN' Parameter!" + "\n" + "No 'Product Names' Table!");
    return; 
    } else {
        Error("TopN");
        Error("'Product Names'");
        return;
    };

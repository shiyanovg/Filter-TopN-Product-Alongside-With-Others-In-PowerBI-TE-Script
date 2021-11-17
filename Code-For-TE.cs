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
    /* This part check for presence of "TopN" What-If parameter and 'Product Names' calculated table */
    Error("No 'TopN' Parameter!" + "\n" + "No 'Product Names' Table!");
    return; 
    } else {
        string test = "Hello";
      
        test.Output();
      
        //return;
    };

    
    FormatDax(Selected.Measures.Expression).Output();

/*    
    
    System.Net.WebClient w = new System.Net.WebClient(); 

string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
string url = "https://raw.githubusercontent.com/microsoft/Analysis-Services/master/BestPracticeRules/BPARules.json";
string downloadLoc = path+@"\TabularEditor\BPARules.json";
w.DownloadFile(url, downloadLoc);
    
    
    
*/ 
    
// ===================

       
/*
Ranking:=
IF (
    ISINSCOPE ( 'Product Names'[Product Name] ),
    VAR ProductsToRank = [TopN Value]
    VAR SalesAmount = [Sales Amount]
    VAR IsOtherSelected =
        SELECTEDVALUE ( 'Product Names'[Product Name] ) = "Others"
    RETURN
        IF (
            IsOtherSelected,
            -- Rank for Others
            ProductsToRank + 1,
            -- Rank for regular products
            IF (
                SalesAmount > 0,
                VAR VisibleProducts =
                    CALCULATETABLE ( VALUES ( 'Product' ), ALLSELECTED ( 'Product Names' ) )
                VAR Ranking =
                    RANKX ( VisibleProducts, [Sales Amount], SalesAmount )
                RETURN
                    IF ( Ranking > 0 && Ranking <= ProductsToRank, Ranking )
            )
        )
 )
*/   
       
       
/*

Visible Row :=
VAR Ranking = [Ranking]
VAR TopNValue = [TopN Value]
VAR Result =
    IF (
        NOT ISBLANK ( Ranking ),
        ( Ranking <= TopNValue ) - ( Ranking = TopNValue + 1 )
    )
RETURN
    Result
    
*/
        
/*
 Sales Amt NA :=
 VAR SalesOfAll =
    CALCULATE ( [Sales Amount], REMOVEFILTERS ( 'Product Names' ) )
RETURN
    IF (
        NOT ISINSCOPE ( 'Product Names'[Product Name] ),
        -- Calculation for a group of products 
        SalesOfAll,
        -- Calculation for one product name
        VAR ProductsToRank = [TopN Value]
        VAR SalesOfCurrentProduct = [Sales Amount]
        VAR IsOtherSelected =
            SELECTEDVALUE ( 'Product Names'[Product Name] ) = "Others"
        RETURN
            IF (
                NOT IsOtherSelected,
                -- Calculation for a regular product
                SalesOfCurrentProduct,
                -- Calculation for Others
                VAR VisibleProducts =
                    CALCULATETABLE (
                        VALUES ( 'Product' ),
                        REMOVEFILTERS ( 'Product Names'[Product Name] )
                    )
                VAR ProductsWithSales =
                    ADDCOLUMNS ( VisibleProducts, "@SalesAmount", [Sales Amount] )
                VAR FilterTopProducts =
                    TOPN ( ProductsToRank, ProductsWithSales, [@SalesAmount] )
                VAR FilterOthers =
                    EXCEPT ( ProductsWithSales, FilterTopProducts )
                VAR SalesOthers =
                    CALCULATE (
                        [Sales Amount],
                        FilterOthers,
                        REMOVEFILTERS ( 'Product Names'[Product Name] )
                    )
                RETURN
                    SalesOthers
            )
    )
 
 */     
        
        
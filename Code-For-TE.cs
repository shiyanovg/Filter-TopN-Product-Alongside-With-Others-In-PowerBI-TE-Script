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
     // IF-part of Ranking expression
   string  PNIsInScope = "IF ( ISINSCOPE ( 'Product Names'[Product Name] ), " ;
   string space = " "; 
   const string quote = "\""; 
   string oth = "Others";
   string PrToR = " VAR ProductsToRank = [TopN Value] ";
   string SA = " VAR SalesAmount = [Sales Amount] ";
   string IsOtherSelected = " VAR IsOtherSelected = SELECTEDVALUE ( 'Product Names'[Product Name] ) = " + quote + oth + quote ;
   string ret = "RETURN";
   string retexpIF = " IF ( IsOtherSelected,  /* Rank for Others */ ProductsToRank + 1 , ";
   string regProds =  " /* Rank for regular products */ IF ( SalesAmount > 0,";
   string VisProds = " VAR VisibleProducts = CALCULATETABLE ( VALUES ( 'Product' ), ALLSELECTED ( 'Product Names' ) ) ";
   string RankVisProds = " VAR Ranking = RANKX ( VisibleProducts, [Sales Amount], SalesAmount ) ";
   string RegProdsReturn = " RETURN IF ( Ranking > 0 && Ranking <= ProductsToRank, Ranking ) ";
   string RegProdsClose = ")"; 
   string retexpIFTrue = retexpIF + space + regProds + space + VisProds + space
   + RankVisProds + space +  RegProdsReturn + space + RegProdsClose ;
   string retexpIFClose = ")";

   // Expression part for TRUE
   string  PNIsInScopeTrue = PrToR + space + SA + space + IsOtherSelected + space
        + ret +  space + retexpIFTrue + retexpIFClose ;
   
   // Expression part for FALSE
   string PNIsInScopeFalse = ")";
   
   // Full expression for Ranking
   string RankMesExp = PNIsInScope + PNIsInScopeTrue + PNIsInScopeFalse;
  
   // Create Ranking measure
   //var RankMes = (Model.Tables["Product Names"] as CalculatedTable).AddMeasure("Ranking 2", FormatDax(RankMesExp) );
    //    RankMes.FormatString = "0";
      
   // Internals for Visible Row
   string VarRanking = " VAR Ranking = [Ranking] " ;
   string VarTopN = " VAR TopNValue = [TopN Value] " ;
   string VarResCondition = " NOT ISBLANK ( Ranking ) " ;
   string VarResTrue = " ( Ranking <= TopNValue ) - ( Ranking = TopNValue + 1 ) " ;
   string VarRes = "VAR Result = IF( " + VarResCondition + ", " + VarResTrue + ")" ;     
   string Res = "Result" ;
   string VisbleRowExpression = VarRanking + space + VarTopN + space + 
        VarRes + space + ret + space + Res ;

    // Create Visible Row Measure
    // var VisbleRowMeasure = (Model.Tables["Product Names"] as CalculatedTable).AddMeasure("Visible Row 2", FormatDax(VisbleRowExpression) );
    
    // Internals for Sales Amt NA




    //return;
};

    
 
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
        
        
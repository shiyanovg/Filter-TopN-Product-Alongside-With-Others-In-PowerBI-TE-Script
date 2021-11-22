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
   var RankMes = (Model.Tables["Product Names"] as CalculatedTable).AddMeasure("Ranking", FormatDax(RankMesExp) );
        RankMes.FormatString = "0";
      
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
     var VisbleRowMeasure = (Model.Tables["Product Names"] as CalculatedTable).AddMeasure("Visible Row", FormatDax(VisbleRowExpression) );
    
    // Internals for Sales Amt NA
    string VarSalesOfAll = "VAR SalesOfAll = CALCULATE ( [Sales Amount], REMOVEFILTERS ( 'Product Names' ) ) " ;
    // RETURN Internals
    string ScopeCheck = " NOT ISINSCOPE ( 'Product Names'[Product Name] ) " ;
    
    
    // Construct TRUE Part for ScopeCheck
    string ScopeCheckTrue = " /* Calculation for a group of products */ SalesOfAll" ;
    
    string VarProdsToRank = " VAR ProductsToRank = [TopN Value] " ;
    string VarSalesCurrProd = " VAR SalesOfCurrentProduct = [Sales Amount] " ;
    
    string RetCondition = " NOT IsOtherSelected " ;
    string RetConditionTrue = " /* Calculation for a regular product */ SalesOfCurrentProduct " ;
    
    string FalseComment = " /* Calculation for Others */ " ;
    string FalseVarVisProds = " VAR VisibleProducts = CALCULATETABLE ( VALUES ( 'Product' ), REMOVEFILTERS ( 'Product Names'[Product Name] ) ) " ;
    string FalseVarProdsWithSales = " VAR ProductsWithSales = ADDCOLUMNS ( VisibleProducts, " + quote + "@SalesAmount" + quote + ", [Sales Amount] ) " ;
    string VarFilterTopProds = " VAR FilterTopProducts = TOPN ( ProductsToRank, ProductsWithSales, [@SalesAmount] ) " ;
    string VarFilterOthers = " VAR FilterOthers = EXCEPT ( ProductsWithSales, FilterTopProducts ) " ;
    string VarSalesOthers = " VAR SalesOthers = CALCULATE ( [Sales Amount], FilterOthers, REMOVEFILTERS ( 'Product Names'[Product Name] ) ) " ;
    string SalOth = " SalesOthers " ;
    
    string RetConditionFalse = 
        FalseComment + space + FalseVarVisProds + space + FalseVarProdsWithSales + space +
             VarFilterTopProds + space + VarFilterOthers + space + VarSalesOthers + space +
             ret + space + SalOth ;
    
    string ReturnPart = 
            "IF( " + RetCondition + ", " 
                    + RetConditionTrue + ", " + 
                    RetConditionFalse + space + ")" ;
    
    // Construct FALSE Part for ScopeCheck
    string ScopeCheckFalse = 
        VarProdsToRank + space + VarProdsToRank + space + VarSalesCurrProd + space + IsOtherSelected + 
        space + ret + space + ReturnPart ;
    
    // Constuct RETURN Part of Sales Amt NA
    string SalesAmtRetExp = "IF( " + ScopeCheck + ", " + ScopeCheckTrue + ", " + ScopeCheckFalse + ")" ; 
    
    // Constuct Sales Amt NA Expression
    string SalesAmtNAExpression = VarSalesOfAll + space + ret + space + SalesAmtRetExp;
    
    //Create Sales Amt NA Measure
    var SalesAmtNAMeasure = (Model.Tables["Product Names"] as CalculatedTable).AddMeasure("Sales Amt NA", FormatDax(SalesAmtNAExpression) );
    SalesAmtNAMeasure.FormatString = "#,0.00";
    
    
};

  
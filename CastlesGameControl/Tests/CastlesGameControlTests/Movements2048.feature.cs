﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.1.0.0
//      SpecFlow Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace CastlesGameControlTests
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.1.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class Movements2048Feature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Movements2048.feature"
#line hidden
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner(null, 0);
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Movements2048", null, ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
        public virtual void TestInitialize()
        {
            if (((testRunner.FeatureContext != null) 
                        && (testRunner.FeatureContext.FeatureInfo.Title != "Movements2048")))
            {
                CastlesGameControlTests.Movements2048Feature.FeatureSetup(null);
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("When_I_Move_Right_And_There_Is_A_Space_On_The_Right_Of_A_Number_It_Is_A_Valid_Mov" +
            "e")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Movements2048")]
        public virtual void When_I_Move_Right_And_There_Is_A_Space_On_The_Right_Of_A_Number_It_Is_A_Valid_Move()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("When_I_Move_Right_And_There_Is_A_Space_On_The_Right_Of_A_Number_It_Is_A_Valid_Mov" +
                    "e", ((string[])(null)));
#line 3
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Column1",
                        "Column2",
                        "Column3",
                        "Column4"});
            table1.AddRow(new string[] {
                        "",
                        "",
                        "",
                        "2"});
            table1.AddRow(new string[] {
                        "",
                        "",
                        "2",
                        ""});
            table1.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
            table1.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
#line 4
testRunner.Given("I have a game board set up as", ((string)(null)), table1, "Given ");
#line 10
testRunner.When("I move Right", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 11
testRunner.Then("it is a valid move", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Column1",
                        "Column2",
                        "Column3",
                        "Column4"});
            table2.AddRow(new string[] {
                        "",
                        "",
                        "",
                        "2"});
            table2.AddRow(new string[] {
                        "",
                        "",
                        "",
                        "2"});
            table2.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
            table2.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
#line 12
testRunner.And("the resultant game board is", ((string)(null)), table2, "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("When_I_Move_Left_And_There_Is_A_Space_On_The_Left_Of_A_Number_It_Is_A_Valid_Move")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Movements2048")]
        public virtual void When_I_Move_Left_And_There_Is_A_Space_On_The_Left_Of_A_Number_It_Is_A_Valid_Move()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("When_I_Move_Left_And_There_Is_A_Space_On_The_Left_Of_A_Number_It_Is_A_Valid_Move", ((string[])(null)));
#line 19
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Column1",
                        "Column2",
                        "Column3",
                        "Column4"});
            table3.AddRow(new string[] {
                        "2",
                        "",
                        "",
                        ""});
            table3.AddRow(new string[] {
                        "",
                        "2",
                        "",
                        ""});
            table3.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
            table3.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
#line 20
testRunner.Given("I have a game board set up as", ((string)(null)), table3, "Given ");
#line 26
testRunner.When("I move Left", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 27
testRunner.Then("it is a valid move", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "Column1",
                        "Column2",
                        "Column3",
                        "Column4"});
            table4.AddRow(new string[] {
                        "2",
                        "",
                        "",
                        ""});
            table4.AddRow(new string[] {
                        "2",
                        "",
                        "",
                        ""});
            table4.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
            table4.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
#line 28
testRunner.And("the resultant game board is", ((string)(null)), table4, "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("When_I_Move_Up_And_There_Is_A_Space_Above_A_Number_It_Is_A_Valid_Move")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Movements2048")]
        public virtual void When_I_Move_Up_And_There_Is_A_Space_Above_A_Number_It_Is_A_Valid_Move()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("When_I_Move_Up_And_There_Is_A_Space_Above_A_Number_It_Is_A_Valid_Move", ((string[])(null)));
#line 35
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "Column1",
                        "Column2",
                        "Column3",
                        "Column4"});
            table5.AddRow(new string[] {
                        "2",
                        "",
                        "",
                        ""});
            table5.AddRow(new string[] {
                        "",
                        "2",
                        "",
                        ""});
            table5.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
            table5.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
#line 36
testRunner.Given("I have a game board set up as", ((string)(null)), table5, "Given ");
#line 42
testRunner.When("I move Up", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 43
testRunner.Then("it is a valid move", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "Column1",
                        "Column2",
                        "Column3",
                        "Column4"});
            table6.AddRow(new string[] {
                        "2",
                        "2",
                        "",
                        ""});
            table6.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
            table6.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
            table6.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
#line 44
testRunner.And("the resultant game board is", ((string)(null)), table6, "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("When_I_Move_Down_And_There_Is_A_Space_Below_A_Number_It_Is_A_Valid_Move")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Movements2048")]
        public virtual void When_I_Move_Down_And_There_Is_A_Space_Below_A_Number_It_Is_A_Valid_Move()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("When_I_Move_Down_And_There_Is_A_Space_Below_A_Number_It_Is_A_Valid_Move", ((string[])(null)));
#line 51
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "Column1",
                        "Column2",
                        "Column3",
                        "Column4"});
            table7.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
            table7.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
            table7.AddRow(new string[] {
                        "2",
                        "",
                        "",
                        ""});
            table7.AddRow(new string[] {
                        "",
                        "2",
                        "",
                        ""});
#line 52
testRunner.Given("I have a game board set up as", ((string)(null)), table7, "Given ");
#line 58
testRunner.When("I move Down", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 59
testRunner.Then("it is a valid move", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                        "Column1",
                        "Column2",
                        "Column3",
                        "Column4"});
            table8.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
            table8.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
            table8.AddRow(new string[] {
                        "",
                        "",
                        "",
                        ""});
            table8.AddRow(new string[] {
                        "2",
                        "2",
                        "",
                        ""});
#line 60
testRunner.And("the resultant game board is", ((string)(null)), table8, "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("When_I_Move_Right_And_There_Is_A_Similar_Number_On_The_Right_Of_A_Number_It_Is_Me" +
            "rged")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Movements2048")]
        public virtual void When_I_Move_Right_And_There_Is_A_Similar_Number_On_The_Right_Of_A_Number_It_Is_Merged()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("When_I_Move_Right_And_There_Is_A_Similar_Number_On_The_Right_Of_A_Number_It_Is_Me" +
                    "rged", ((string[])(null)));
#line 67
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table9 = new TechTalk.SpecFlow.Table(new string[] {
                        "Column1",
                        "Column2",
                        "Column3",
                        "Column4"});
            table9.AddRow(new string[] {
                        "",
                        "",
                        "",
                        "2"});
            table9.AddRow(new string[] {
                        "4",
                        "",
                        "2",
                        ""});
            table9.AddRow(new string[] {
                        "",
                        "2",
                        "",
                        ""});
            table9.AddRow(new string[] {
                        "",
                        "4",
                        "4",
                        ""});
#line 68
testRunner.Given("I have a game board set up as", ((string)(null)), table9, "Given ");
#line 74
testRunner.When("I move Right", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 75
testRunner.Then("it is a valid move", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            TechTalk.SpecFlow.Table table10 = new TechTalk.SpecFlow.Table(new string[] {
                        "Column1",
                        "Column2",
                        "Column3",
                        "Column4"});
            table10.AddRow(new string[] {
                        "",
                        "",
                        "",
                        "2"});
            table10.AddRow(new string[] {
                        "",
                        "",
                        "4",
                        "2"});
            table10.AddRow(new string[] {
                        "",
                        "",
                        "",
                        "2"});
            table10.AddRow(new string[] {
                        "",
                        "",
                        "",
                        "8"});
#line 76
testRunner.And("the resultant game board is", ((string)(null)), table10, "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion

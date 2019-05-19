using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbUp.Oracle;
using Shouldly;
using Xunit;

namespace DbUp.Tests.Support.Oracle
{
    public class OracleConnectionManagerTests
    {
        [Fact]
        public void CanParseSingleLineScript()
        {
            const string singleCommand = "create table FOO (myid INT NOT NULL);";

            var connectionManager = new OracleConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(singleCommand);

            result.Count().ShouldBe(1);
        }

        [Fact]
        public void CanParseMultilineScript()
        {
            var multiCommand = "create table FOO (myid INT NOT NULL);";
            multiCommand += Environment.NewLine;
            multiCommand += "create table BAR (myid INT NOT NULL);";

            var connectionManager = new OracleConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(multiCommand);

            result.Count().ShouldBe(2);
        }

        [Fact]
        public void CanParseBlockTerminatorSingleStatement()
        {
            var singleCommand = "create table FOO (myid INT NOT NULL);";
            singleCommand += Environment.NewLine;
            singleCommand += "/";
            
            var connectionManager = new OracleConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(singleCommand);

            result.Count().ShouldBe(1);

        }
        
        [Fact]
        public void CanParseBlockTerminatorOnlyForBlankLine()
        {
            var singleCommand = "create table FOO (myid INT NOT NULL);/";
            singleCommand += Environment.NewLine;
            singleCommand += "/";
            
            var connectionManager = new OracleConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(singleCommand);

            result.Count().ShouldBe(1);
        }      
        
        [Fact]
        public void CanParseBlockTerminatorMultipleStatements()
        {
            var multiCommand = "create table FOO (myid INT NOT NULL);/";
            multiCommand += Environment.NewLine;
            multiCommand += "/";
            multiCommand += Environment.NewLine;
            multiCommand += @"SELECT *
FROM user_tables
WHERE table_name ='FOO'";        // NOTE: leaving off ';'
            multiCommand += Environment.NewLine;
            multiCommand += "/";
            multiCommand += Environment.NewLine;
            multiCommand += "create table BAR (myid INT NOT NULL);";
            multiCommand += Environment.NewLine;
            multiCommand += "/";
            multiCommand += Environment.NewLine;
            multiCommand += @"SELECT *
FROM user_tables
WHERE table_name ='BAR';";        // NOTE: including ';'

            var connectionManager = new OracleConnectionManager("connectionstring");
            var result = connectionManager.SplitScriptIntoCommands(multiCommand);

            result.Count().ShouldBe(4);
        }  
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlReceiptReader
{
    class SQL
    {

        public static string CreateDatabaseScript()
        {
            string sql = @"BEGIN TRANSACTION;
                            CREATE TABLE IF NOT EXISTS 'receipts' (
	                            'id'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	                            'uid'	TEXT NOT NULL,
	                            'amount'	TEXT NOT NULL,
	                            'receiptnumber'	TEXT,
	                            'intreceiptnumber'	TEXT,
	                            'date'	TEXT,
	                            'mod'	TEXT,
	                            'paycard'	TEXT,
	                            'paycash'	TEXT,
                                'paystr'	TEXT,
	                            'payvmp'	TEXT,
	                            'xmldata'	TEXT,
	                            'receipttype'	TEXT,
	                            'invoicenumber'	TEXT,
	                            'taxbasebasic'	TEXT,
	                            'taxbasereduced'	TEXT,
	                            'basicvatamount'	TEXT,
	                            'reducedvatamount'	TEXT,
	                            'taxfreeamount'	TEXT
                            );
                            CREATE TABLE IF NOT EXISTS 'unsentreceipts' (
	                            'id'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	                            'date'	TEXT NOT NULL,
	                            'uid'	TEXT NOT NULL,
	                            'state'	TEXT NOT NULL,
	                            'print'	TEXT NOT NULL
                            );
                            CREATE TABLE IF NOT EXISTS 'items' (
	                            'id'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	                            'receiptid'	INTEGER NOT NULL,
	                            'name'	TEXT,
                                'mj'	TEXT,
	                            'unitprice'	TEXT,
	                            'price'	TEXT,
	                            'vatrate'	TEXT,
	                            'quantity'	INTEGER,
	                            'refid'	TEXT
                            );
                            CREATE TABLE IF NOT EXISTS 'environment' (
	                            'id'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	                            'name'	TEXT NOT NULL,
	                            'value'	TEXT,
	                            'enabled'	TEXT
                            );
                            INSERT INTO 'environment' ('id','name','value','enabled') VALUES (0,'Vystavil','Predavac 1','true'),
                             (1,'DriverPass','',''),
                             (2,'EnablePohoda','','false'),
                             (3,'PohodaIn','C:/eKasa_test/in.xml',''),
                             (4,'PohodaOut','C:/eKasa_test/out.xml',''),
                             (5,'ProtocolID','1074E0DF-CA98-4DD2-8E6C-6CEF88506C18',NULL),
                             (6,'DeveloperMode','','false'),
                             (7,'OnStartup','','false'),
                             (8,'InitSetup','','true'),
                             (9,'Zakaznik','','false'),
                             (10,'AfterFooter','Ďakujeme za nákup','false'),
                             (11,'LineLength','48',''),
                             (12,'IUInitSetup','','false'),
                             (13,'AUInitSetup','','false'),
                             (14,'Servis','','false');
                            COMMIT;";

            return sql;
        }
    }
}

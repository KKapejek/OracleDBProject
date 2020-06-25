using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.IO;
using System.Xml;

namespace generator
{
    public partial class MainWindow : Window
    {
        OracleConnection con = null;
        string tableContext = "";

        int bedsID = 0, cabinetsID = 0, chairsID = 0, clientsID = 0, employeesID = 0, invoicesID = 0, materialsID = 0, shiftsID = 0, shippingsID = 0, tablesID = 0;

        string[] randomNames = {
            "Piotr", "Krzysztof", "Andrzej", "Tomasz", "Jan", "Paweł", "Michał", "Marcin", "Stanisław", "Jakub", "Adam", "Marek", "Łukasz", "Grzegorz", "Mateusz", "Wojciech", "Mariusz", "Dariusz", "Zbigniew", "Jerzy",
            "Anna", "Maria", "Katarzyna", "Małgorzata", "Agnieszka", "Barbara", "Ewa", "Krystyna", "Elżbieta", "Magdalena", "Joanna", "Zofia", "Aleksandra", "Monika", "Teresa", "Danuta", "Natalia", "Karolina", "Marta", "Julia"
        };
        string[] randomSurnames = { "Nowak", "Kowalski", "Wiśniewski", "Wójcik", "Kowalczyk", "Kamiński", "Lewandowski", "Zieliński", "Woźniak", "Szymański", "Dąbrowski", "Mazur", "Jankowski", "Kwiatkowski", "Wojciechowski", "Krawczyk", "Kaczmarek", "Piotrowski", "Grabowski" };
        string[] randomAddress = { "Warszawa", "Kraków", "Łódź", "Wrocław", "Poznań", "Gdańsk", "Szczecin", "Bydgoszcz", "Lublin", "Białystok" };
        string[] randomMaterials = { "drewno", "marmur", "stal", "kamień", "plastik", "granit", "piaskowiec", "guma", "aluminium", "karton", "brąz", "srebro", "złoto", "platyna", "magia" };

        string[] tablesList = { "EMPLOYEES", "SHIFTS", "CLIENTS", "INVOICES", "SHIPPINGS", "MATERIALS", "BEDS", "CABINETS", "CHAIRS", "TABLES" };

        DateTime dateOfShifts = new DateTime(2020, 3, 1, 8, 0, 0);
        DateTime dateOfSale = new DateTime(2020, 1, 1, 14, 0, 0);

        public MainWindow()
        {
            this.setConnection();
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            tableContext = (string)(sender as RadioButton).Content;
        }

        private void setConnection()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            con = new OracleConnection(connectionString);
            try
            {
                con.Open();
            }
            catch (Exception e) { MessageBox.Show(e.ToString()); }
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int insertCount = Int32.Parse(Count.Text);
                if (tableContext != "")
                    if (insertCount > 0)
                    {
                        insertIntoTables();
                    }
                    else
                        MessageBox.Show("Enter a number greater than 0.");
                else
                    MessageBox.Show("Select a table.");
            }
            catch (FormatException)
            {
                MessageBox.Show("Enter a number.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void insertIntoTables()
        {
            Random random = new Random();
            bool insertedVariables = true;
            int insertCount = Int32.Parse(Count.Text);

            string command = "INSERT ALL ";
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;

            switch (tableContext)
            {
                case "EMPLOYEES":
                    for (int i = 0; i < insertCount; i++)
                    {
                        command += "INTO EMPLOYEES (ID, NAME, SURNAME, PAY) VALUES ('" + ++employeesID + "', '" + randomNames[random.Next(randomNames.Count())] + "', '" + randomSurnames[random.Next(randomSurnames.Count())] + "', '" + random.Next(1500, 3001) + "')\n";
                    }
                    break;

                case "SHIFTS":
                    if (employeesID > 0)
                        for (int i = 0; i < insertCount; i++)
                        {
                            command += "INTO SHIFTS (ID, ID_EMPLOYEE, DATE_OF_SHIFT) VALUES ('" + ++shiftsID + "', '" + random.Next(1, employeesID + 1) + "', TO_DATE('" + dateOfShifts.ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM/DD/YYYY HH24:MI:SS'))\n";

                            dateOfShifts = dateOfShifts.AddDays(1);
                        }
                    else
                    {
                        MessageBox.Show("Database is missing table entries for: EMPLOYEES");
                        insertedVariables = false;
                    }
                    break;

                case "CLIENTS":
                    for (int i = 0; i < insertCount; i++)
                    {
                        command += "INTO CLIENTS (ID, NAME, SURNAME, ADDRESS) VALUES ('" + ++clientsID + "', '" + randomNames[random.Next(randomNames.Count())] + "', '" + randomSurnames[random.Next(randomSurnames.Count())] + "', '" + randomAddress[random.Next(randomAddress.Count())] + "')\n";
                    }
                    break;

                case "INVOICES":
                    if (clientsID > 0)
                        for (int i = 0; i < insertCount; i++)
                        {
                            command += "INTO INVOICES (ID, ID_CLIENT, DATEOFSALE, PRICEOFSALE) VALUES ('" + ++invoicesID + "', '" + random.Next(1, clientsID + 1) + "', TO_DATE('" + dateOfSale.ToString("MM/dd/yyyy HH:mm:ss") + "', 'MM/DD/YYYY HH24:MI:SS'), '" + random.Next(1, 1001) + "')\n";

                            dateOfSale.AddDays(1);
                        }
                    else
                    {
                        MessageBox.Show("Database is missing table entries for: CLIENTS");
                        insertedVariables = false;
                    }
                    break;

                case "SHIPPINGS":
                    if (invoicesID > 0 && clientsID > 0)
                        for (int i = 0; i < insertCount; i++)
                        {
                            command += "INTO SHIPPINGS  (ID, ID_INVOICE, ID_CLIENT) VALUES ('" + ++shippingsID + "', '" + random.Next(1, invoicesID + 1) + "', '" + random.Next(1, clientsID + 1) + "')\n";
                        }
                    else
                    {
                        MessageBox.Show("Database is missing table entries for: INVOICES and/or CLIENTS");
                        insertedVariables = false;
                    }
                    break;

                case "MATERIALS":
                    for (int i = 0; i < insertCount; i++)
                    {
                        string randomizedMaterial = randomMaterials[random.Next(1, randomMaterials.Count())] + "-" + randomMaterials[random.Next(1, randomMaterials.Count())];
                        command += "INTO MATERIALS (ID, MATERIAL) VALUES ('" + ++materialsID + "', '" + randomizedMaterial + "')\n";
                    };
                    break;

                case "BEDS":
                    if (materialsID > 0 && employeesID > 0 && invoicesID > 0)
                        for (int i = 0; i < insertCount; i++)
                        {
                            command += "INTO BEDS (ID, ID_MATERIAL, QUALITY, ID_EMPLOYEE, PRICE, ID_INVOICE) VALUES ('" + ++bedsID + "', '" + random.Next(1, materialsID + 1) + "', '" + random.Next(101) + "', '" + random.Next(1, employeesID + 1) + "', '" + random.Next(1001) + "', '" + random.Next(1, invoicesID + 1) + "')\n";
                        }
                    else
                    {
                        MessageBox.Show("Database is missing table entries for: MATERIALS and/or EMPLOYEESS and/or INVOICES");
                        insertedVariables = false;
                    }
                    break;

                case "CABINETS":
                    if (materialsID > 0 && employeesID > 0 && invoicesID > 0)
                        for (int i = 0; i < insertCount; i++)
                        {
                            command += "INTO CABINETS (ID, ID_MATERIAL, QUALITY, ID_EMPLOYEE, PRICE, ID_INVOICE) VALUES ('" + ++cabinetsID + "', '" + random.Next(1, materialsID + 1) + "', '" + random.Next(101) + "', '" + random.Next(1, employeesID + 1) + "', '" + random.Next(1001) + "', '" + random.Next(1, invoicesID + 1) + "')\n";
                        }
                    else
                    {
                        MessageBox.Show("Database is missing table entries for: MATERIALS and/or EMPLOYEESS and/or INVOICES");
                        insertedVariables = false;
                    }
                    break;

                case "CHAIRS":
                    if (materialsID > 0 && employeesID > 0 && invoicesID > 0)
                        for (int i = 0; i < insertCount; i++)
                        {
                            command += "INTO CHAIRS (ID, ID_MATERIAL, QUALITY, ID_EMPLOYEE, PRICE, ID_INVOICE) VALUES ('" + ++chairsID + "', '" + random.Next(1, materialsID + 1) + "', '" + random.Next(101) + "', '" + random.Next(1, employeesID + 1) + "', '" + random.Next(1001) + "', '" + random.Next(1, invoicesID + 1) + "')\n";
                        }
                    else
                    {
                        MessageBox.Show("Database is missing table entries for: MATERIALS and/or EMPLOYEESS and/or INVOICES");
                        insertedVariables = false;
                    }
                    break;

                case "TABLES":
                    if (materialsID > 0 && employeesID > 0 && invoicesID > 0)
                        for (int i = 0; i < insertCount; i++)
                        {
                            command += "INTO TABLES (ID, ID_MATERIAL, QUALITY, ID_EMPLOYEE, PRICE, ID_INVOICE) VALUES ('" + ++tablesID + "', '" + random.Next(1, materialsID + 1) + "', '" + random.Next(101) + "', '" + random.Next(1, employeesID + 1) + "', '" + random.Next(1001) + "', '" + random.Next(1, invoicesID + 1) + "')\n";
                        }
                    else
                    {
                        MessageBox.Show("Database is missing table entries for: MATERIALS and/or EMPLOYEESS and/or INVOICES");
                        insertedVariables = false;
                    }
                    break;

                default:
                    MessageBox.Show("No table selected");
                    insertedVariables = false;
                    break;
            }

            if (insertedVariables)
            {
                command += "SELECT 1 FROM DUAL";
                cmd.CommandText = command;
                try
                {
                    cmd.ExecuteNonQuery();

                    string fileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\INSERT.txt";
                    addToFile(fileName, command);

                    MessageBox.Show("Inserted " + insertCount + " entries into " + tableContext);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString());
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (string selectTable in tablesList)
                {
                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandText = "DELETE FROM " + selectTable;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                int sumOfElements = bedsID + cabinetsID + chairsID + clientsID + employeesID + invoicesID + materialsID + shiftsID + shippingsID + tablesID;
                MessageBox.Show("Deleted " + sumOfElements + " elements from database.");

                bedsID = 0;
                cabinetsID = 0;
                chairsID = 0;
                clientsID = 0;
                employeesID = 0;
                invoicesID = 0;
                materialsID = 0;
                shiftsID = 0;
                shippingsID = 0;
                tablesID = 0;

                dateOfShifts = new DateTime(2020, 3, 1, 8, 0, 0);
                dateOfSale = new DateTime(2020, 1, 1, 14, 0, 0);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            XmlTextReader reader = new XmlTextReader("C:/Users/Krzysiek/Desktop/EXPORT.xml");

            OracleCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;

            string tableString = "";
            string command = "INSERT ALL ";

            int bedsID = 0, cabinetsID = 0, chairsID = 0, clientsID = 0, employeesID = 0, invoicesID = 0, materialsID = 0, shiftsID = 0, shippingsID = 0, tablesID = 0;
            int entries = 0;
            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            tableString = reader.Name;
                            switch (tableString)
                            {
                                case "BEDS":
                                    bedsID++;
                                    command += "INTO BEDS (ID, ID_MATERIAL, QUALITY, ID_EMPLOYEE, PRICE, ID_INVOICE) VALUES ('";
                                    break;
                                case "CABINETS":
                                    cabinetsID++;
                                    command += "INTO CABINETS (ID, ID_MATERIAL, QUALITY, ID_EMPLOYEE, PRICE, ID_INVOICE) VALUES ('";
                                    break;
                                case "CHAIRS":
                                    chairsID++;
                                    command += "INTO CHAIRS (ID, ID_MATERIAL, QUALITY, ID_EMPLOYEE, PRICE, ID_INVOICE) VALUES ('";
                                    break;
                                case "CLIENTS":
                                    clientsID++;
                                    command += "INTO CLIENTS (ID, NAME, SURNAME, ADDRESS) VALUES ('";
                                    break;
                                case "EMPLOYEES":
                                    employeesID++;
                                    command += "INTO EMPLOYEES (ID, NAME, SURNAME, PAY) VALUES ('";
                                    break;
                                case "INVOICES":
                                    invoicesID++;
                                    command += "INTO INVOICES (ID, ID_CLIENT, DATEOFSALE, PRICEOFSALE) VALUES ('";
                                    break;
                                case "MATERIALS":
                                    materialsID++;
                                    command += "INTO MATERIALS (ID, MATERIAL) VALUES ('";
                                    break;
                                case "SHIFTS":
                                    shiftsID++;
                                    command += "INTO SHIFTS (ID, ID_EMPLOYEE, DATE_OF_SHIFT) VALUES ('";
                                    break;
                                case "SHIPPINGS":
                                    shippingsID++;
                                    command += "INTO SHIPPINGS  (ID, ID_INVOICE, ID_CLIENT) VALUES ('";
                                    break;
                                case "TABLES":
                                    tablesID++;
                                    command += "INTO TABLES (ID, ID_MATERIAL, QUALITY, ID_EMPLOYEE, PRICE, ID_INVOICE) VALUES ('";
                                    break;
                                case "DATE_OF_SHIFT":
                                case "DATEOFSALE":
                                    command = command.Remove(command.Length - 1, 1);
                                    command += "TO_DATE('";
                                    break;
                            }
                            break;

                        case XmlNodeType.Text:
                            if (tableString.Equals("DATE_OF_SHIFT") || tableString.Equals("DATEOFSALE"))
                            {
                                command += reader.Value.Remove(reader.Value.Length - 6, 6) + "', '";
                            }
                            else
                            {
                                command += reader.Value + "', '";
                            }
                            break;

                        case XmlNodeType.EndElement:
                            if (!reader.Name.Equals("NewDataSet"))
                            {
                                if (reader.Name.Equals("DATE_OF_SHIFT") || reader.Name.Equals("DATEOFSALE"))
                                {
                                    command += "YYYY-MM-DD\"T\"HH24:MI:SS'), '";
                                }

                                if (!reader.Name.Equals(tableString))
                                {
                                    command = command.Remove(command.Length - 3, 3);
                                    command += ")";
                                    entries++;
                                }
                            }
                            break;
                    }
                }
                try
                {
                    command += "SELECT 1 FROM DUAL";
                    cmd.CommandText = command;
                    cmd.ExecuteNonQuery();

                    dateOfShifts = dateOfShifts.AddDays(shiftsID);
                    dateOfSale.AddDays(invoicesID);

                    this.bedsID += bedsID;
                    this.cabinetsID += cabinetsID;
                    this.chairsID += chairsID;
                    this.clientsID += clientsID;
                    this.employeesID += employeesID;
                    this.invoicesID += invoicesID;
                    this.materialsID += materialsID;
                    this.shiftsID += shiftsID;
                    this.shippingsID += shippingsID;
                    this.tablesID += tablesID;

                    MessageBox.Show("Inserted " + entries + " elements into the database");
                }
                catch
                {
                    MessageBox.Show("Unable to import file into database");
                }
            }
            catch
            {
                MessageBox.Show("EXPORT.xml file not found.");
            }
            
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int i = 0;
                DataSet dataSet = new DataSet();
                foreach (string selectTable in tablesList)
                {
                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandText = "SELECT * FROM " + selectTable;
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataAdapter dataAdapter = new OracleDataAdapter())
                    {
                        dataAdapter.SelectCommand = cmd;
                        dataAdapter.Fill(dataSet);
                        dataSet.Tables[i++].TableName = selectTable;
                    }
                }
                addToFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\EXPORT.xml", dataSet.GetXml());
                MessageBox.Show("Successfully exported database to EXPORT.xml");
            }
            catch (Exception exce)
            {
                MessageBox.Show(exce.ToString());
            }
        }

        private void addToFile(string fileName, string text)
        {
            File.AppendAllLines(fileName, new string[] { text });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            getInfoFromDatabase();
        }

        private void getInfoFromDatabase()
        {
            foreach (string selectTable in tablesList)
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT COALESCE(MAX(ID),0) FROM " + selectTable;
                cmd.CommandType = CommandType.Text;
                int selectID = Int32.Parse(cmd.ExecuteScalar().ToString());

                switch (selectTable)
                {
                    case "BEDS":
                        bedsID = selectID;
                        break;
                    case "CABINETS":
                        cabinetsID = selectID;
                        break;
                    case "CHAIRS":
                        chairsID = selectID;
                        break;
                    case "CLIENTS":
                        clientsID = selectID;
                        break;
                    case "EMPLOYEES":
                        employeesID = selectID;
                        break;
                    case "INVOICES":
                        invoicesID = selectID;
                        break;
                    case "MATERIALS":
                        materialsID = selectID;
                        break;
                    case "SHIFTS":
                        shiftsID = selectID;
                        break;
                    case "SHIPPINGS":
                        shippingsID = selectID;
                        break;
                    case "TABLES":
                        tablesID = selectID;
                        break;
                    default:
                        break;
                }

                dateOfSale = dateOfSale.AddDays(invoicesID);
                dateOfShifts = dateOfShifts.AddDays(shiftsID);
            }
            int sumOfElements = bedsID + cabinetsID + chairsID + clientsID + employeesID + invoicesID + materialsID + shiftsID + shippingsID + tablesID;
            MessageBox.Show("Loaded " + sumOfElements + " elements from database.");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            con.Close();
        }
    }
}
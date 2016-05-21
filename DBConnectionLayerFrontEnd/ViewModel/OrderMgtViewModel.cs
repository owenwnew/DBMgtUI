using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Data;
using System.ComponentModel;
using System.IO;

using System.Text.RegularExpressions;
using DBConnectionLayer;
using DBConnectionLayerFrontEnd.Commands;
using DBConnectionLayerFrontEnd.Resource;
using DBConnectionLayerFrontEnd.View;
using DBConnectionLayerFrontEnd.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Office.Interop;
using Word = Microsoft.Office.Interop.Word;

namespace DBConnectionLayerFrontEnd.ViewModel
{
    public class OrderMgtViewModel: WorkSpacesViewModel
    {
        ObservableCollection<InvoiceItemListModel> _invoiceItemList;
        ObservableCollection<WorkSpacesViewModel> _workspaces;
        CommandBase _CreateNewList;
        CommandBase _SaveListToDB;
        CommandBase _CreateWordDoc;
        InvoiceItemListViewModel _itemListViewModel;
        ConnectToMongoDB _connectedMongo = new ConnectToMongoDB();
        //int generatedInvoiceNumber = 3000;

        public OrderMgtViewModel()
        {

        }

        public void createNewList()
        {
            InvoiceItemListViewModel invoiceItemListViewModel = this.Workspaces.Where(vm => vm.DisplayName == "Invoice List").FirstOrDefault() as InvoiceItemListViewModel;

            if (invoiceItemListViewModel != null)
                this.Workspaces.Remove(invoiceItemListViewModel);
            invoiceItemListViewModel = initiateItemListViewModel(invoiceItemListViewModel);
            this.Workspaces.Add(invoiceItemListViewModel);
            this.SetActiveWorkspace(invoiceItemListViewModel);
            clearStatus();
        }

        private InvoiceItemListViewModel initiateItemListViewModel(InvoiceItemListViewModel ItemListViewModel)
        {
            _itemListViewModel = new InvoiceItemListViewModel();
            _itemListViewModel.InvoiceList = populateEmptyList();
            return _itemListViewModel;
        }

        ObservableCollection<InvoiceItemListModel> populateEmptyList ()
        {
            ObservableCollection<InvoiceItemListModel> EmptyList = new ObservableCollection<InvoiceItemListModel>();
            try
            {
                EmptyList.Add(new InvoiceItemListModel("", "", "", "", ""));
                return EmptyList;
            }
            catch(Exception ex)
            {
                return EmptyList;
            }

        }


        public void saveListToDB()
        {
            string invoiceNumber = "";

            invoiceNumber = generateInvoiceNumber();

            var invoiceEntity = createInvoiceDBEntity(_itemListViewModel.InvoiceList, invoiceNumber);
            
            _connectedMongo.insertDocumentToDB(invoiceEntity, "OrderMgtCollection");

            //updateInvoiceDBEntity(_itemListViewModel.InvoiceList, invoiceNumber); //in the future replace this with createBsonArray at insert stage to save a step.

            saveStatus = "Invoice Saved";
            OnPropertyChanged("saveStatus");

            invoiceID = Convert.ToString(invoiceNumber);
            OnPropertyChanged("invoiceID");
        }   

        public static BsonArray CreateBsonArray(ObservableCollection<InvoiceItemListModel> invoiceItemList)
        {
            BsonArray createdCollectionBsonArray = new BsonArray();
            BsonArray combinedBsonArray = new BsonArray();

            for (int i = 0; i < invoiceItemList.Count(); i++)
            {
                combinedBsonArray.Add(new BsonArray {"Item:", invoiceItemList[i].item });
                combinedBsonArray.Add(new BsonArray {"Description", invoiceItemList[i].description});
                combinedBsonArray.Add(new BsonArray {"Quantity", invoiceItemList[i].quantity });
                combinedBsonArray.Add(new BsonArray {"Unit Price", invoiceItemList[i].unitPrice });
                combinedBsonArray.Add(new BsonArray {"Total Item Price", invoiceItemList[i].totalPrice });
                //combinedBsonArray.Add(new BsonArray {"Payment Option", invoiceItemList[i].paymentOption });
                
            }


            createdCollectionBsonArray.Add(combinedBsonArray);
            return createdCollectionBsonArray;
        }

        public BsonDocument createInvoiceDBEntity(ObservableCollection<InvoiceItemListModel> invoiceList, string generatedInvoiceNumber)
        {
            BsonArray insertArray = new BsonArray();
            insertArray =  CreateBsonArray(_itemListViewModel.InvoiceList);
            var document = new BsonDocument {
                { "Invoice Number" , Convert.ToString(generatedInvoiceNumber)},
                { "Customer Name", customerName},
                { "Invoiced Date", date },
                { "HST", hST},
                { "Discount", discount },
                { "Invoice Detail", insertArray}

            };

            return document;
        }
        
        public void updateInvoiceDBEntity(ObservableCollection<InvoiceItemListModel> invoiceList, int generatedInvoiceNumber)
        {
            for(int i = 1; i < invoiceList.Count(); i++)
            {
                var document = new BsonDocument {
                    { "Item", invoiceList[i].item},
                    { "Item Description", invoiceList[i].description},
                    //{ "Item Catagory", invoiceList[i].itemCatagory},
                    { "Item Unit Price", invoiceList[i].unitPrice},
                    { "Item Total Price", invoiceList[i].totalPrice},
                    //{ "Item Payment", invoiceList[i].paymentOption},
                };

                _connectedMongo.updateDocumentInDB(document, "OrderMgtCollection", "Invoice Number", Convert.ToString(generatedInvoiceNumber), "Invoice Detail");

            };

            



        }

        public void createInvoiceWordDoc(string pathName)
        {

            string fileName = "Invoice " + invoiceID + ".docx";
            using (WordprocessingDocument package = WordprocessingDocument.Create(pathName  + fileName, WordprocessingDocumentType.Document))
            {
                package.AddMainDocumentPart();
                //Body body = package.MainDocumentPart.Document.AppendChild(new Body());
                //Body body = package.MainDocumentPart.Document.Body;

                //Paragraph para = body.AppendChild(new Paragraph());
                //Run run = para.AppendChild(new Run());

                //run.AppendChild(new Text("P2SO-" + invoiceID));

                package.MainDocumentPart.Document = new Document(
                    new Body(
                        new Paragraph(
                            new Run(
                                new Text("P2SO - "+invoiceID)
                                )
                            )
                        )                   
                    );
                package.MainDocumentPart.Document.Save();


                package.Close();

            }

            



        }

        public void editInvoiceWordDocTemplate(string pathName)
        {
            
            string sourceFile = Path.Combine(pathName, "InvoiceTemplate.docx");
            string destinationFile = Path.Combine(pathName, "Invoice "+ invoiceID.ToString() + ".docx");
            
            File.Copy(sourceFile,destinationFile,true);
            using (WordprocessingDocument document = WordprocessingDocument.Open(destinationFile, true))
            {
                MainDocumentPart mainPart = document.MainDocumentPart;

                Body body = mainPart.Document.Body;
                
                #region customer Info table
                TableProperties customerInfoTblprop = new TableProperties(
                    new TableBorders(
                        new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 }),
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    //new TableStyle() { Val = "TableGrid" },
                    new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct });

                Table customerInfoTable = new Table();
                List<string> customerInfoTableData = new List<string>();
                customerInfoTable.AppendChild<TableProperties>((TableProperties)customerInfoTblprop.Clone());
                customerInfoTableData.Add("Customer: " +this.customerName);
                customerInfoTableData.Add("SALES RECEIPT");
                customerInfoTable = AppendCustomerTableInfo(customerInfoTableData, customerInfoTable, false);


                customerInfoTableData = new List<string>();
                customerInfoTableData.Add("Email Address:" + this.emailAddress);
                customerInfoTableData.Add("Invoice P2SO-" + invoiceID);
                customerInfoTable = AppendCustomerTableInfo(customerInfoTableData, customerInfoTable, false);

                customerInfoTableData = new List<string>();
                customerInfoTableData.Add("Contact: " + this.phoneNum);
                customerInfoTableData.Add("Date: " + this.date);
                customerInfoTable = AppendCustomerTableInfo(customerInfoTableData, customerInfoTable, false);
                body.Append(customerInfoTable);
                #endregion

                body.Append(new Paragraph(new Run(new Text("\n"))));

                #region Invoice Item List Table
                TableProperties tblprop = new TableProperties(
                    new TableBorders( 
                        new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size= 0, Space = 0 },
                        new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 }
                        ),
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center},
                    //new TableStyle() { Val = "TableGrid"},
                    new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct}
                    );

                Table invoiceTable = new Table();
                List<string> tableData = new List<string>();
                invoiceTable.AppendChild<TableProperties>((TableProperties)tblprop.Clone());
                tableData.Add("Item");
                tableData.Add("Description");
                tableData.Add("Quantity");
                tableData.Add("Unit Price");
                tableData.Add("Item Total price");
                invoiceTable = AppendInvoiceTableInfo(tableData, invoiceTable, true);

                tableData = new List<string>();
                tableData.Add(this._itemListViewModel.InvoiceList[0].item);
                tableData.Add(this._itemListViewModel.InvoiceList[0].description);
                tableData.Add(this._itemListViewModel.InvoiceList[0].quantity);
                tableData.Add(this._itemListViewModel.InvoiceList[0].unitPrice);
                tableData.Add(this._itemListViewModel.InvoiceList[0].totalPrice);
                invoiceTable = AppendInvoiceTableInfo(tableData, invoiceTable, false);

                body.Append(invoiceTable);

                #endregion

                body.Append(new Paragraph(new Run(new Text("\n"))));

                #region total price output


                double itemSubTotal = 0;
                for (int i = 0; i < this._itemListViewModel.InvoiceList.Count; i++)
                {
                    itemSubTotal += Math.Round(Convert.ToDouble(this._itemListViewModel.InvoiceList[i].totalPrice), 2);
                }

                double totalPrice = 0;
                double hstAmount = 0;

                totalPrice = Math.Round(itemSubTotal * (1 + Convert.ToDouble(this.hST) / 100), 2);
                hstAmount = Math.Round(itemSubTotal * Convert.ToDouble(this.hST) / 100, 2);


                TableProperties totalPriceTableProp = new TableProperties(
                    new TableBorders(
                        new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 }),    
                    new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                    new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct });

                Table totalPriceTable = new Table();

                totalPriceTable.AppendChild<TableProperties>((TableProperties)totalPriceTableProp.Clone());

                List<string> totalPriceTableData = new List<string>();
                totalPriceTableData.Add("Sub Total:");
                totalPriceTableData.Add(itemSubTotal.ToString("N2"));
                totalPriceTable = AppendTotalPriceTableInfo(totalPriceTableData, totalPriceTable, false);


                totalPriceTableData = new List<string>();
                totalPriceTableData.Add("HST:");
                totalPriceTableData.Add(hstAmount.ToString("N2"));
                totalPriceTable = AppendTotalPriceTableInfo(totalPriceTableData, totalPriceTable, false);

                totalPriceTableData = new List<string>();
                totalPriceTableData.Add("Total:");
                totalPriceTableData.Add(totalPrice.ToString("N2"));
                totalPriceTable = AppendTotalPriceTableInfo(totalPriceTableData, totalPriceTable, false);

                totalPriceTableData = new List<string>();
                totalPriceTableData.Add("Paid:");
                totalPriceTableData.Add(this.paidAmount);
                totalPriceTable = AppendTotalPriceTableInfo(totalPriceTableData, totalPriceTable, false);

                totalPriceTableData = new List<string>();
                totalPriceTableData.Add("Balance:");
                totalPriceTableData.Add((totalPrice - Convert.ToDouble(this.paidAmount)).ToString("N2"));
                totalPriceTable = AppendTotalPriceTableInfo(totalPriceTableData, totalPriceTable, false);
                body.Append(totalPriceTable);




                //Paragraph totalPricePara = new Paragraph();
                //ParagraphProperties totalPriceParaP = new ParagraphProperties();
                //Justification pricePositionJustification = new Justification() { Val = JustificationValues.Right };
                //totalPriceParaP.Append(pricePositionJustification);


                //Run totalPriceRun = new Run();
                //RunProperties totalPriceRunP = new RunProperties();
                //FontSize totalPriceFontSize = new FontSize() { Val = "22" };
                //Color totalPriceFontColor = new Color() { Val = "365F91" };
                //MarginHeight totalPriceMargin = new MarginHeight() { Val = 0 };
                //Text subTotalPriceLine = new Text("Sub Toal: " + itemSubTotal.ToString("N2"));
                //Text HSTAmountLine = new Text("HST: " + hstAmount.ToString("N2"));
                //Text totalPriceLine = new Text("Total: " + totalPrice.ToString("N2"));
                //Text paidAmount = new Text("Paid: ");
                //Text balance = new Text("Balance: " );
                //totalPriceRunP.Append(totalPriceFontSize);
                //totalPriceRunP.Append(totalPriceFontColor);
                //totalPriceRunP.Append(totalPriceMargin);
                //totalPriceRun.Append(totalPriceRunP);
                //totalPriceRun.Append(subTotalPriceLine);
                //totalPriceRun.Append(new Break());
                //totalPriceRun.Append(HSTAmountLine);
                //totalPriceRun.Append(new Break());
                //totalPriceRun.Append(totalPriceLine);
                //totalPriceRun.Append(new Break());
                //totalPriceRun.Append(paidAmount);
                //totalPriceRun.Append(new Break());
                //totalPriceRun.Append(balance);


                //totalPricePara.Append(totalPriceParaP);
                //totalPricePara.Append(totalPriceRun);
                //body.Append(totalPricePara);

                
                #endregion

                document.Close();
                
                
            }
            Word.Application wordApp = new Word.Application();
            
            wordApp.Documents.Open(pathName + @"\"+"Invoice " + invoiceID.ToString() + ".docx");

            wordApp.Visible = true;

        }


        public Table AppendInvoiceTableInfo(List<string> tableData, Table table, bool header)
        {

            TableRow tr = new TableRow();
            PreviousTablePropertyExceptions ptpex = new PreviousTablePropertyExceptions();
            TableCellMarginDefault tcm = new TableCellMarginDefault();
            tcm.TopMargin = new TopMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };

            tcm.BottomMargin = new BottomMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };
            ptpex.Append(tcm);
            tr.Append(ptpex);

            TableCell tc;




            TableCellProperties tcp = new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center });
            tcp.TableCellMargin = new TableCellMargin(new RightMargin() { Type = TableWidthUnitValues.Pct, Width = "50" });
            tcp.TableCellMargin.LeftMargin = new LeftMargin() { Type = TableWidthUnitValues.Pct, Width = "50" };
            tcp.TableCellMargin.TopMargin = new TopMargin() { Type = TableWidthUnitValues.Pct, Width = "1" };



            ParagraphProperties ppp = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });
            ppp.Append(new KeepLines());
            ppp.Append(new KeepNext());
            SpacingBetweenLines sp = new SpacingBetweenLines();
            sp.After = "0";
            ppp.Append(sp);




            if (!header)
            {
                RunProperties rp = new RunProperties(new Bold() { Val = false });
                Shading shading = new Shading();
                rp.Bold.Val = false;
                rp.RunFonts = new RunFonts() { Ascii = "Calibri" };
                rp.FontSize = new FontSize() { Val = new StringValue("22") };

                shading = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "e3e6e7" };

                tcp.Shading = shading;
                for (int i = 0; i < tableData.Count; i++)
                {
                    tc = new TableCell();
                    //TableStyle tableStyle = new TableStyle() { Val = "TableGrid" };

                    // Make the table width 100% of the page width.
                    TableWidth tableWidth = new TableWidth() { Width = "50000", Type = TableWidthUnitValues.Auto };
                    tcp.Append(tableWidth);
                    tc.Append((TableCellProperties)tcp.Clone());

                    tc.Append((ParagraphProperties)ppp.Clone());

                    Run r = new Run();
                    r.PrependChild<RunProperties>((RunProperties)rp.Clone());
                    r.Append(new Text(tableData[i].ToString()));
                    tc.Append(new Paragraph(r));
                    tr.Append(tc);
                }
            }
            else
            {
                RunProperties rp = new RunProperties(new Bold() { Val = true }, new TabChar());
                rp.Bold.Val = true;
                rp.RunFonts = new RunFonts() { Ascii = "Arial" };
                rp.FontSize = new FontSize() { Val = new StringValue("20") };
                //Color color = new Color() { Val = "365F91", ThemeColor = ThemeColorValues.Accent1, ThemeShade = "BF" };
                Shading shading = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "a7c6d7" };
                //ppp.Shading = shading;

                tcp.TableCellMargin.TopMargin.Width = "200";
                tcp.Shading = shading;

                for (int i = 0; i < tableData.Count; i++)
                {
                    tc = new TableCell();
                    TableStyle tableStyle = new TableStyle() { Val = "TableGrid" };

                    // Make the table width 100% of the page width.
                    TableWidth tableWidth = new TableWidth() { Width = "50000", Type = TableWidthUnitValues.Auto };
                    tcp.Append(tableStyle, tableWidth);
                    tc.Append((TableCellProperties)tcp.Clone());
                    tc.Append((ParagraphProperties)ppp.Clone());


                    Run r = new Run();
                    r.PrependChild<RunProperties>((RunProperties)rp.Clone());
                    r.Append(new Text(tableData[i].ToString()));
                    tc.Append(new Paragraph(r));
                    tr.Append(tc);

                }
            }

            
            table.Append(tr);

            return table;
            

        }

        public Table AppendCustomerTableInfo(List<string> tableData, Table table, bool header)
        {

            TableRow tr = new TableRow();
            PreviousTablePropertyExceptions ptpex = new PreviousTablePropertyExceptions();
            TableCellMarginDefault tcm = new TableCellMarginDefault();
            tcm.TopMargin = new TopMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };

            tcm.BottomMargin = new BottomMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };
            ptpex.Append(tcm);
            tr.Append(ptpex);

            TableCell tc;




            TableCellProperties tcp = new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center });
            tcp.TableCellMargin = new TableCellMargin(new RightMargin() { Type = TableWidthUnitValues.Pct, Width = "50" });
            tcp.TableCellMargin.LeftMargin = new LeftMargin() { Type = TableWidthUnitValues.Pct, Width = "50" };
            tcp.TableCellMargin.TopMargin = new TopMargin() { Type = TableWidthUnitValues.Pct, Width = "1" };
            tcp.TableCellBorders = new TableCellBorders(
                        new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 }
                );


            ParagraphProperties ppl = new ParagraphProperties(new Justification() { Val = JustificationValues.Left });
            ParagraphProperties ppr = new ParagraphProperties(new Justification() { Val = JustificationValues.Right });
            ppl.Append(new KeepLines());
            ppl.Append(new KeepNext());
            SpacingBetweenLines sp = new SpacingBetweenLines();
            sp.After = "0";
            ppl.Append(sp);

            ppr.Append(new KeepLines());
            ppr.Append(new KeepNext());
            SpacingBetweenLines spr = new SpacingBetweenLines();
            spr.After = "0";
            ppr.Append(spr);



            RunProperties rp = new RunProperties(new Bold() { Val = false });
                Shading shading = new Shading();
                rp.Bold.Val = false;
                rp.RunFonts = new RunFonts() { Ascii = "Calibri" };
                rp.FontSize = new FontSize() { Val = new StringValue("26") };

                shading = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto" };

                tcp.Shading = shading;
                for (int i = 0; i < tableData.Count; i++)
                {
                    tc = new TableCell();
                    //TableStyle tableStyle = new TableStyle() { Val = "TableGrid" };

                    // Make the table width 100% of the page width.
                    TableWidth tableWidth = new TableWidth() { Width = "50000", Type = TableWidthUnitValues.Auto };
                    //tcp.Append(tableStyle, tableWidth);
                    tcp.Append(tableWidth);
                    tc.Append((TableCellProperties)tcp.Clone());
                    if(i == 0 || i == 2 || i == 4)
                        tc.Append((ParagraphProperties)ppl.Clone());
                    else
                        tc.Append((ParagraphProperties)ppr.Clone());

                    Run r = new Run();
                    r.PrependChild<RunProperties>((RunProperties)rp.Clone());
                    r.Append(new Text(tableData[i].ToString()));
                    tc.Append(new Paragraph(r));
                    tr.Append(tc);
                }
            


            table.Append(tr);

            return table;

        }

        public Table AppendTotalPriceTableInfo(List<string> tableData, Table table, bool header)
        {

            TableRow tr = new TableRow();
            PreviousTablePropertyExceptions ptpex = new PreviousTablePropertyExceptions();
            TableCellMarginDefault tcm = new TableCellMarginDefault();
            tcm.TopMargin = new TopMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };

            tcm.BottomMargin = new BottomMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };
            ptpex.Append(tcm);
            tr.Append(ptpex);

            TableCell tc;




            TableCellProperties tcp = new TableCellProperties(new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center });
            tcp.TableCellMargin = new TableCellMargin(new RightMargin() { Type = TableWidthUnitValues.Pct, Width = "50" });
            tcp.TableCellMargin.LeftMargin = new LeftMargin() { Type = TableWidthUnitValues.Pct, Width = "50" };
            tcp.TableCellMargin.TopMargin = new TopMargin() { Type = TableWidthUnitValues.Pct, Width = "1" };
            tcp.TableCellBorders = new TableCellBorders(
                        new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 },
                        new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.None), Size = 0, Space = 0 }
                );


            ParagraphProperties ppl = new ParagraphProperties(new Justification() { Val = JustificationValues.Left });
            ParagraphProperties ppr = new ParagraphProperties(new Justification() { Val = JustificationValues.Right });
            ppl.Append(new KeepLines());
            ppl.Append(new KeepNext());
            SpacingBetweenLines sp = new SpacingBetweenLines();
            sp.After = "0";
            ppl.Append(sp);

            ppr.Append(new KeepLines());
            ppr.Append(new KeepNext());
            SpacingBetweenLines spr = new SpacingBetweenLines();
            spr.After = "0";
            ppr.Append(spr);



            RunProperties rp = new RunProperties(new Bold() { Val = false });
            Shading shading = new Shading();
            rp.Bold.Val = false;
            rp.RunFonts = new RunFonts() { Ascii = "Calibri" };
            rp.FontSize = new FontSize() { Val = new StringValue("26") };

            shading = new Shading() { Val = ShadingPatternValues.Clear, Color = "auto" };

            tcp.Shading = shading;
            for (int i = 0; i < tableData.Count; i++)
            {
                tc = new TableCell();
                //TableStyle tableStyle = new TableStyle() { Val = "TableGrid" };

                // Make the table width 100% of the page width.
                TableWidth tableWidth = new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Dxa };
                //tcp.Append(tableStyle, tableWidth);
                tcp.Append(tableWidth);
                
                if (i == 0 || i == 2 || i == 4)
                {
                    tcp.TableCellWidth = new TableCellWidth() { Width = "4900", Type = TableWidthUnitValues.Dxa };
                    tc.Append((ParagraphProperties)ppr.Clone());
                    tc.Append((TableCellProperties)tcp.Clone());
                }
                else
                {
                    tcp.TableCellWidth = new TableCellWidth() { Width = "100", Type = TableWidthUnitValues.Dxa };
                    tc.Append((ParagraphProperties)ppl.Clone());
                    tc.Append((TableCellProperties)tcp.Clone());
                }

                Run r = new Run();
                r.PrependChild<RunProperties>((RunProperties)rp.Clone());
                r.Append(new Text(tableData[i].ToString()));
                tc.Append(new Paragraph(r));
                tr.Append(tc);
            }



            table.Append(tr);

            return table;

        }


        public string generateInvoiceNumber ()
        {
            int generatedInvoiceNumber = 300;
            double numOfDigits = 0;
            string returnString;
            generatedInvoiceNumber += _connectedMongo.numberOfDocumentsInCollection("OrderMgtCollection");

            returnString = generatedInvoiceNumber.ToString();

            if (generatedInvoiceNumber != 0)
                numOfDigits = Math.Floor(Math.Log10(generatedInvoiceNumber) + 1);

            for(int i = Convert.ToInt16(numOfDigits); i < 6; i++)
            {

                returnString = "0" + returnString;

            }

            return returnString;

        }

        public void clearStatus()
        {
            saveStatus = "";
            OnPropertyChanged("saveStatus");
            invoiceID = "";
            OnPropertyChanged("invoiceID");
        }

        public string customerName { get; set;}
        public string date { get; set; }
        public string hST { get; set; }
        public string discount { get; set; }

        public string saveStatus { get; set; }
        public string invoiceID { get; set; }

        public string phoneNum { get; set; }
        public string emailAddress { get; set; }
        public string paidAmount { get; set; }

        #region ICommands
        public ICommand CreateList
        {
            get
            {
                if (_CreateNewList == null)
                {
                    //commandBase(Action<object> executeDelegate, Predicate<object> canExecuteDelegate)
                    //this means commandBase takes 2 object parameters to create constructor
                    //first it will see if this command can be executed by going to CanUpdate
                    //if it cannot execute, it will disable the button
                    //once it gets a true boolean value, it will then proceed to execute
                    //if it can execute: then go to action boject which is updateTextOnCommand()
                    //_updateCommand = new CommandBase(param => this.UpdateTextOnCommand(), Param => this.CanUpdate);
                    _CreateNewList = new CommandBase(param => this.createNewList());
                }
                return _CreateNewList;
            }


        }

        public ICommand SaveListToDB
        {
            get
            {
                if (_SaveListToDB == null)
                {
                    _SaveListToDB = new CommandBase(param => this.saveListToDB());
                }
                return _SaveListToDB;
            }


        }

        public ICommand CreateInvoiceWordDocument
        {
            get
            {
                if (_CreateWordDoc == null)
                {
                    _CreateWordDoc = new CommandBase(param => this.editInvoiceWordDocTemplate("C:\\CommonRepo\\DBConnectionLayer\\Resource"));
                }
                return _CreateWordDoc;
            }


        }


        #endregion

        #region mini workspace

        public ObservableCollection<WorkSpacesViewModel> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<WorkSpacesViewModel>();
                    _workspaces.CollectionChanged += this.OnWorkspacesChanged;
                }

                return _workspaces;
            }
        }

        void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkSpacesViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkspaceRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkSpacesViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }

        void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkSpacesViewModel workspace = sender as WorkSpacesViewModel;
            workspace.Dispose();
            this.Workspaces.Remove(workspace);
        }

        void SetActiveWorkspace(WorkSpacesViewModel workspace)
        {
            //Debug.Assert(this.Workspaces.Contains(workspace));

            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }

        #endregion
    }
}

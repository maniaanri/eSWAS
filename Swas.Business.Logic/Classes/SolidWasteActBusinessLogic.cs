﻿namespace Swas.Business.Logic.Classes
{
    using Common;
    using Data.Entity;
    using Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SolidWasteActBusinessLogic : BusinessLogicBase
    {
        public SolidWasteActBusinessLogic()
            : base()
        {
        }

        public List<SolidWasteActInfoItem> Load()
        {
            var result = new List<SolidWasteActInfoItem>();

            try
            {
                Connect();

                result = (from solidWasteAct in Context.SolidWasteActs
                          join landfill in Context.Landfills on solidWasteAct.LandfillId equals landfill.Id
                          join receiver in Context.Receivers on solidWasteAct.ReceiverId equals receiver.Id
                          join customer in Context.Customers on solidWasteAct.CustomerId equals customer.Id
                          join solidWasteActDetail in Context.SolidWasteActDetails on solidWasteAct.Id equals solidWasteActDetail.SolidWasteActId into LeftJoinSolidWasteActDetail
                          from solidWasteActDetail in LeftJoinSolidWasteActDetail.DefaultIfEmpty()
                          group solidWasteActDetail by new
                          {
                              Id = solidWasteAct.Id,
                              ActDate = solidWasteAct.ActDate,
                              CusotmerCode = customer.Code,
                              CustomerName = customer.Name,
                              LandfillName = landfill.Name,
                              ReceiverName = receiver.Name,
                              ReceiverLastName = receiver.LastName
                          } into Group
                          select new SolidWasteActInfoItem
                          {
                              Id = Group.Key.Id,
                              ActDate = Group.Key.ActDate,
                              CustomerCode = Group.Key.CusotmerCode,
                              CustomerName = Group.Key.CustomerName,
                              LandfillName = Group.Key.LandfillName,
                              ReceiverName = Group.Key.ReceiverName,
                              ReceiverLastName = Group.Key.ReceiverLastName,
                              Quantity = Group.Sum(a => a == null ? 0 : a.Quantity),
                              Price = Group.Sum(a => a == null ? 0 : a.Amount)
                          }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Dispose();
            }

            return result;
        }

        private void Validate(SolidWasteActItem item)
        {
            #region Validtaion

            if (item.Customer == null)
                item.Customer = new CustomerItem
                {
                    Code = string.Empty,
                    ContactInfo = string.Empty,
                    Name = string.Empty,
                };

            if (item.Representative == null)
                item.Representative = new RepresentativeItem
                {
                    Name = string.Empty,
                };

            if (item.Transporter == null)
                item.Transporter = new TransporterItem
                {
                    CarModel = string.Empty,
                    CarNumber = string.Empty,
                    DriverInfo = string.Empty,
                };

            if (item.Position == null)
                item.Position = new PositionItem
                {
                    Name = string.Empty
                };

            if (item.Receiver == null)
                item.Receiver = new ReceiverItem
                {
                    Name = string.Empty,
                    LastName = string.Empty,
                };

            if (string.IsNullOrEmpty(item.Remark))
                item.Remark = string.Empty;
            #endregion Validation
        }

        public void Create(SolidWasteActItem item)
        {
            try
            {
                Connect();

                Validate(item);

                #region Customer

                var customerInfo = (from customer in Context.Customers
                                    where customer.Code.Trim() == item.Customer.Code.Trim()
                                    select customer).FirstOrDefault();
                if (customerInfo == null)
                {
                    customerInfo = new Customer()
                    {
                        Type = (int)item.Customer.Type,
                        Code = item.Customer.Code.Trim(),
                        Name = item.Customer.Name.Trim(),
                        ContactInfo = item.Customer.ContactInfo.Trim(),
                    };

                    Context.Customers.Add(customerInfo);
                    Context.SaveChanges();
                }
                else
                {
                    customerInfo.Name = item.Customer.Name.Trim();
                    customerInfo.ContactInfo = item.Customer.ContactInfo.Trim();
                }

                #endregion Customer

                #region Representative

                var representativeInfo = (from representative in Context.Representatives
                                          where representative.Name.Trim() == item.Representative.Name.Trim()
                                          select representative).FirstOrDefault();

                if (representativeInfo == null)
                {
                    representativeInfo = new Representative()
                    {
                        Name = item.Representative.Name.Trim()
                    };

                    Context.Representatives.Add(representativeInfo);
                    Context.SaveChanges();
                }

                #endregion Representative

                #region Tranporter

                var transporterInfo = (from transporter in Context.Transporters
                                       where transporter.CarModel.Trim() == item.Transporter.CarModel.Trim() &&
                                             transporter.CarNumber.Trim() == item.Transporter.CarNumber.Trim() &&
                                             transporter.DriverInfo.Trim() == item.Transporter.DriverInfo.Trim()
                                       select transporter).FirstOrDefault();

                if (transporterInfo == null)
                {
                    transporterInfo = new Transporter
                    {
                        CarModel = item.Transporter.CarModel.Trim(),
                        CarNumber = item.Transporter.CarNumber.Trim(),
                        DriverInfo = item.Transporter.DriverInfo.Trim()
                    };

                    Context.Transporters.Add(transporterInfo);
                }

                #endregion

                #region CustomerRepresentative

                var customerRepresentativeInfo = (from customerRepresentative in Context.CustomerRepresentatives
                                                  where customerRepresentative.CustomerId == customerInfo.Id &&
                                                        customerRepresentative.RepresentativeId == representativeInfo.Id &&
                                                        customerRepresentative.TransporterId == transporterInfo.Id
                                                  select new
                                                  {
                                                      Id = customerRepresentative.Id
                                                  }).FirstOrDefault();

                if (customerRepresentativeInfo == null)
                {
                    Context.CustomerRepresentatives.Add(new CustomerRepresentative
                    {
                        CustomerId = customerInfo.Id,
                        RepresentativeId = representativeInfo.Id,
                        TransporterId = transporterInfo.Id
                    });

                    Context.SaveChanges();
                }

                #endregion CustomerRepresentative

                #region Receiver

                var receiverInfo = (from receiver in Context.Receivers
                                    where receiver.Name.Trim() == item.Receiver.Name &&
                                          receiver.LastName.Trim() == item.Receiver.LastName.Trim()
                                    select receiver).FirstOrDefault();

                if (receiverInfo == null)
                {
                    receiverInfo = new Receiver
                    {
                        Name = item.Receiver.Name,
                        LastName = item.Receiver.LastName
                    };

                    Context.Receivers.Add(receiverInfo);
                }

                #endregion Reciever

                #region Position

                var positionInfo = (from position in Context.Positions
                                    where position.Name.Trim() == item.Position.Name
                                    select position).FirstOrDefault();

                if (positionInfo == null)
                {
                    positionInfo = new Position
                    {
                        Name = item.Position.Name.Trim()
                    };

                    Context.Positions.Add(positionInfo);
                    Context.SaveChanges();
                }

                #endregion Position

                #region ReceiverPosition

                var receiverPositionInfo = (from receiverPosition in Context.ReceiverPositions
                                            where receiverPosition.ReceiverId == receiverInfo.Id && receiverPosition.PositionId == positionInfo.Id
                                            select new
                                            {
                                                ID = receiverPosition.Id
                                            }).FirstOrDefault();

                if (receiverPositionInfo == null)
                {
                    Context.ReceiverPositions.Add(new ReceiverPosition
                    {
                        ReceiverId = receiverInfo.Id,
                        PositionId = positionInfo.Id
                    });

                    Context.SaveChanges();
                }

                #endregion ReceiverPosition

                #region SolidWasteActs

                var newItem = new SolidWasteAct
                {
                    LandfillId = item.LandfillId,
                    ActDate = item.ActDate,
                    CreateDate = DateTime.Now,
                    ReceiverId = receiverInfo.Id,
                    PositionId = positionInfo.Id,
                    CustomerId = customerInfo.Id,
                    RepresentativeId = representativeInfo.Id,
                    TransporterId = transporterInfo.Id,
                    Remark = item.Remark
                };

                Context.SolidWasteActs.Add(newItem);
                Context.SaveChanges();

                #endregion SolidWasteActs

                #region SolidWasteActDetails

                foreach (var detailItem in item.SolidWasteActDetails)
                    Context.SolidWasteActDetails.Add(new SolidWasteActDetail
                    {
                        SolidWasteActId = newItem.Id,
                        WasteTypeId = detailItem.WasteTypeId,
                        Quantity = detailItem.Quantity,
                        UnitPrice = detailItem.UnitPrice,
                        Amount = detailItem.Amount,
                    });

                Context.SaveChanges();

                #endregion SolidWasteActDetails
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Dispose();
            }
        }

        public SolidWasteActHelperDataItem Get(int Id)
        {

            var result = new SolidWasteActHelperDataItem()
            {
                CustomerItemSource = new List<CustomerSearchItem>(),
                EditorItem = new SolidWasteActItem(),
                LandfillItemSource = new List<LandfillItem>(),
                RecieverItemSource = new List<ReceiverPositionSearchItem>(),
                TransporterItemSource = new List<TransporterSearchItem>(),
                WasteTypeItemSource = new List<WasteTypeSmartItem>()
            };

            try
            {
                Connect();

                result.EditorItem = (from solidWasteAct in Context.SolidWasteActs
                                     join customer in Context.Customers on solidWasteAct.CustomerId equals customer.Id
                                     join receiver in Context.Receivers on solidWasteAct.ReceiverId equals receiver.Id
                                     join position in Context.Positions on solidWasteAct.PositionId equals position.Id
                                     join representative in Context.Representatives on solidWasteAct.RepresentativeId equals representative.Id
                                     join transporter in Context.Transporters on solidWasteAct.TransporterId equals transporter.Id
                                     where solidWasteAct.Id == Id
                                     select new SolidWasteActItem
                                     {
                                         Id = solidWasteAct.Id,
                                         ActDate = solidWasteAct.ActDate,
                                         LandfillId = solidWasteAct.LandfillId,
                                         CustomerId = customer.Id,
                                         ReceiverId = receiver.Id,
                                         PositionId = position.Id,
                                         RepresentativeId = representative.Id,
                                         TransporterId = transporter.Id,
                                         Remark = solidWasteAct.Remark,
                                         Customer = new CustomerItem
                                         {
                                             Id = customer.Id,
                                             Type = (CustomerType)customer.Type,
                                             Name = customer.Name,
                                             Code = customer.Code,
                                             ContactInfo = customer.ContactInfo,

                                         },
                                         Receiver = new ReceiverItem
                                         {
                                             Id = receiver.Id,
                                             Name = receiver.Name,
                                             LastName = receiver.LastName
                                         },
                                         Position = new PositionItem
                                         {
                                             Id = position.Id,
                                             Name = position.Name
                                         },
                                         Representative = new RepresentativeItem
                                         {
                                             Id = representative.Id,
                                             Name = representative.Name,
                                         },
                                         Transporter = new TransporterItem
                                         {
                                             Id = transporter.Id,
                                             CarModel = transporter.CarModel,
                                             CarNumber = transporter.CarNumber,
                                             DriverInfo = transporter.DriverInfo
                                         },
                                     }).FirstOrDefault();

                if (result != null && result.EditorItem != null)
                {
                    var resource = LoadHellperItemSource(result.EditorItem.Customer.Type);
                    if (resource != null)
                    {
                        result.CustomerItemSource = resource.CustomerItemSource;
                        result.RecieverItemSource = resource.RecieverItemSource;
                        result.TransporterItemSource = resource.TransporterItemSource;
                        result.WasteTypeItemSource = resource.WasteTypeItemSource;
                        result.LandfillItemSource = resource.LandfillItemSource;
                    }

                    result.EditorItem.SolidWasteActDetails = (from solidWasteActDetail in Context.SolidWasteActDetails
                                                              join wasteType in Context.WasteTypes on solidWasteActDetail.WasteTypeId equals wasteType.Id
                                                              where solidWasteActDetail.SolidWasteActId == Id
                                                              select new SolidWasteActDetailItem
                                                              {
                                                                  Id = solidWasteActDetail.Id,
                                                                  WasteTypeId = solidWasteActDetail.WasteTypeId,
                                                                  WasteTypeName = wasteType.Name,
                                                                  Amount = solidWasteActDetail.Amount,
                                                                  Quantity = solidWasteActDetail.Quantity,
                                                                  UnitPrice = solidWasteActDetail.UnitPrice
                                                              }).ToList();
                }
                else
                    result.EditorItem = new SolidWasteActItem
                    {
                        ActDate = DateTime.Now,
                        SolidWasteActDetails = new List<SolidWasteActDetailItem>()
                    };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Dispose();
            }

            return result;
        }

        public void Edit(SolidWasteActItem item)
        {
            try
            {
                Connect();
                Validate(item);

                #region Customer

                var customerInfo = (from customer in Context.Customers
                                    where customer.Code.Trim() == item.Customer.Code.Trim()
                                    select customer).FirstOrDefault();
                if (customerInfo == null)
                {
                    customerInfo = new Customer()
                    {
                        Type = (int)item.Customer.Type,
                        Code = item.Customer.Code.Trim(),
                        Name = item.Customer.Name.Trim(),
                        ContactInfo = item.Customer.ContactInfo.Trim(),
                    };

                    Context.Customers.Add(customerInfo);
                    Context.SaveChanges();
                }
                else
                {
                    customerInfo.Name = item.Customer.Name.Trim();
                    customerInfo.ContactInfo = item.Customer.ContactInfo.Trim();
                }

                #endregion Customer

                #region Representative

                var representativeInfo = (from representative in Context.Representatives
                                          where representative.Name.Trim() == item.Representative.Name.Trim()
                                          select representative).FirstOrDefault();

                if (representativeInfo == null)
                {
                    representativeInfo = new Representative()
                    {
                        Name = item.Representative.Name.Trim()
                    };

                    Context.Representatives.Add(representativeInfo);
                    Context.SaveChanges();
                }

                #endregion Representative

                #region Tranporter

                var transporterInfo = (from transporter in Context.Transporters
                                       where transporter.CarModel.Trim() == item.Transporter.CarModel.Trim() &&
                                             transporter.CarNumber.Trim() == item.Transporter.CarNumber.Trim() &&
                                             transporter.DriverInfo.Trim() == item.Transporter.DriverInfo.Trim()
                                       select transporter).FirstOrDefault();

                if (transporterInfo == null)
                {
                    transporterInfo = new Transporter
                    {
                        CarModel = item.Transporter.CarModel.Trim(),
                        CarNumber = item.Transporter.CarNumber.Trim(),
                        DriverInfo = item.Transporter.DriverInfo.Trim()
                    };

                    Context.Transporters.Add(transporterInfo);
                }

                #endregion

                #region CustomerRepresentative

                var customerRepresentativeInfo = (from customerRepresentative in Context.CustomerRepresentatives
                                                  where customerRepresentative.CustomerId == customerInfo.Id &&
                                                        customerRepresentative.RepresentativeId == representativeInfo.Id &&
                                                        customerRepresentative.TransporterId == transporterInfo.Id
                                                  select new
                                                  {
                                                      Id = customerRepresentative.Id
                                                  }).FirstOrDefault();

                if (customerRepresentativeInfo == null)
                {
                    Context.CustomerRepresentatives.Add(new CustomerRepresentative
                    {
                        CustomerId = customerInfo.Id,
                        RepresentativeId = representativeInfo.Id,
                        TransporterId = transporterInfo.Id
                    });

                    Context.SaveChanges();
                }

                #endregion CustomerRepresentative

                #region Receiver

                var receiverInfo = (from receiver in Context.Receivers
                                    where receiver.Name.Trim() == item.Receiver.Name &&
                                          receiver.LastName.Trim() == item.Receiver.LastName.Trim()
                                    select receiver).FirstOrDefault();

                if (receiverInfo == null)
                {
                    receiverInfo = new Receiver
                    {
                        Name = item.Receiver.Name,
                        LastName = item.Receiver.LastName
                    };

                    Context.Receivers.Add(receiverInfo);
                }

                #endregion Reciever

                #region Position

                var positionInfo = (from position in Context.Positions
                                    where position.Name.Trim() == item.Position.Name
                                    select position).FirstOrDefault();

                if (positionInfo == null)
                {
                    positionInfo = new Position
                    {
                        Name = item.Position.Name.Trim()
                    };

                    Context.Positions.Add(positionInfo);
                    Context.SaveChanges();
                }

                #endregion Position

                #region ReceiverPosition

                var receiverPositionInfo = (from receiverPosition in Context.ReceiverPositions
                                            where receiverPosition.ReceiverId == receiverInfo.Id && receiverPosition.PositionId == positionInfo.Id
                                            select new
                                            {
                                                ID = receiverPosition.Id
                                            }).FirstOrDefault();

                if (receiverPositionInfo == null)
                {
                    Context.ReceiverPositions.Add(new ReceiverPosition
                    {
                        ReceiverId = receiverInfo.Id,
                        PositionId = positionInfo.Id
                    });

                    Context.SaveChanges();
                }

                #endregion ReceiverPosition

                #region SolidWasteActs

                var updateItem = (from solidWasteAct in Context.SolidWasteActs
                                  where solidWasteAct.Id == item.Id
                                  select solidWasteAct).FirstOrDefault();

                if (updateItem == null)
                    throw new Exception("Record Not Found");

                updateItem.LandfillId = item.LandfillId;
                updateItem.ActDate = item.ActDate;
                updateItem.CreateDate = DateTime.Now;
                updateItem.ReceiverId = receiverInfo.Id;
                updateItem.PositionId = positionInfo.Id;
                updateItem.CustomerId = customerInfo.Id;
                updateItem.RepresentativeId = representativeInfo.Id;
                updateItem.TransporterId = transporterInfo.Id;
                updateItem.Remark = item.Remark.Trim();

                #endregion SolidWasteActs

                #region SolidWasteActDetails

                var deletedDetailItemSource = (from detail in Context.SolidWasteActDetails
                                               where detail.SolidWasteActId == item.Id
                                               select detail).ToList();

                foreach (var it in deletedDetailItemSource)
                    Context.SolidWasteActDetails.Remove(it);

                foreach (var detailItem in item.SolidWasteActDetails)
                    Context.SolidWasteActDetails.Add(new SolidWasteActDetail
                    {
                        SolidWasteActId = item.Id,
                        WasteTypeId = detailItem.WasteTypeId,
                        Quantity = detailItem.Quantity,
                        UnitPrice = detailItem.UnitPrice,
                        Amount = detailItem.Amount,
                    });

                Context.SaveChanges();

                #endregion SolidWasteActDetails

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Dispose();
            }
        }

        public void Remove(int solidWasteActId)
        {
            try
            {
                Connect();

                var deleteDetailItemSource = (from solidWasteActDetail in Context.SolidWasteActDetails
                                              where solidWasteActDetail.SolidWasteActId == solidWasteActId
                                              select solidWasteActDetail).ToList();

                var deleteSolidWasteActItem = (from solidWasteAct in Context.SolidWasteActs
                                               where solidWasteAct.Id == solidWasteActId
                                               select solidWasteAct).FirstOrDefault();

                if (deleteSolidWasteActItem != null)
                {
                    foreach (var item in deleteDetailItemSource)
                        Context.SolidWasteActDetails.Remove(item);

                    Context.SolidWasteActs.Remove(deleteSolidWasteActItem);
                    Context.SaveChanges();
                }
                else
                    throw new Exception("ჩანაწერი ვერ მოიძებნა");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Dispose();
            }
        }

        private SolidWasteActHelperDataItem LoadHellperItemSource(CustomerType customerType)
        {
            var result = new SolidWasteActHelperDataItem
            {
                LandfillItemSource = new List<LandfillItem>(),
                RecieverItemSource = new List<ReceiverPositionSearchItem>(),
                CustomerItemSource = new List<CustomerSearchItem>()
            };

            var landfillItemSource = (from region in Context.Regions
                                      join landfill in Context.Landfills on region.Id equals landfill.RegionId
                                      orderby region.Name ascending
                                      select new
                                      {
                                          Id = landfill.Id,
                                          LandfillName = landfill.Name,
                                          RegionName = region.Name
                                      }).ToList();

            foreach (var item in landfillItemSource)
                result.LandfillItemSource.Add(new LandfillItem
                {
                    Id = item.Id,
                    Name = string.Format("{0} - {1}", item.RegionName.Trim(), item.LandfillName.Trim())
                });

            result.WasteTypeItemSource = (from wasteType in Context.WasteTypes
                                          select new WasteTypeSmartItem
                                          {
                                              Id = wasteType.Id,
                                              Name = wasteType.Name,
                                          }).ToList();

            #region Reciever

            var recieverItemSource = (from receiverPosition in Context.ReceiverPositions
                                      join receiver in Context.Receivers on receiverPosition.ReceiverId equals receiver.Id
                                      join position in Context.Positions on receiverPosition.PositionId equals position.Id
                                      select new
                                      {
                                          receiver.Name,
                                          receiver.LastName,
                                          PositionName = position.Name
                                      }).ToList();

            result.RecieverItemSource = (from item in recieverItemSource
                                         select new ReceiverPositionSearchItem
                                         {
                                             Description = String.Format("{0} {1} {2}", item.Name.Trim(), item.LastName.Trim(), item.PositionName.Trim()),
                                             ReceiverName = item.Name,
                                             ReceiverLastName = item.LastName,
                                             Posistion = item.PositionName
                                         }).ToList();




            #endregion

            #region Customer

            var searchCustomerItemSource = (from customerRepresentative in Context.CustomerRepresentatives
                                            join customer in Context.Customers on customerRepresentative.CustomerId equals customer.Id
                                            join representative in Context.Representatives on customerRepresentative.RepresentativeId equals representative.Id
                                            join transporter in Context.Transporters on customerRepresentative.TransporterId equals transporter.Id
                                            where customer.Type == (int)customerType
                                            select new
                                            {
                                                Name = customer.Name,
                                                Code = customer.Code,
                                                ContactInfo = customer.ContactInfo,
                                                RepresentativeName = representative.Name,
                                                CarNumber = transporter.CarNumber,
                                                CarModel = transporter.CarModel,
                                                DriverInfo = transporter.DriverInfo,
                                            }).ToList();

            result.CustomerItemSource = (from item in searchCustomerItemSource
                                         select new CustomerSearchItem
                                         {
                                             Description = String.Format("{0} - {1}, {2}, {3}, {4}, {5} ", item.Code.Trim(), item.Name.Trim(), item.RepresentativeName.Trim(), item.CarNumber, item.CarModel, item.DriverInfo),
                                             Name = item.Name,
                                             Code = item.Code,
                                             ContactInfo = item.ContactInfo,
                                             RepresentativeName = item.RepresentativeName,
                                             CarNumber = item.CarNumber,
                                             CarModel = item.CarModel,
                                             DriverInfo = item.DriverInfo,
                                         }).ToList();

            #endregion Customer

            #region Transporter

            var searchTransporterItemSource = (from transporter in Context.Transporters
                                               select new
                                               {
                                                   CarNumber = transporter.CarNumber,
                                                   CarModel = transporter.CarModel,
                                                   DriverInfo = transporter.DriverInfo,
                                               }).ToList();

            result.TransporterItemSource = (from item in searchTransporterItemSource
                                            select new TransporterSearchItem
                                            {
                                                Description = String.Format("{0} - {1}, {2}", item.CarNumber.Trim(), item.CarModel.Trim(), item.DriverInfo),
                                                CarNumber = item.CarNumber,
                                                CarModel = item.CarModel,
                                                DriverInfo = item.DriverInfo,
                                            }).ToList();


            #endregion Transporter
            return result;
        }

        public SolidWasteActHelperDataItem LoadHellperSource(CustomerType customerType)
        {
            var result = new SolidWasteActHelperDataItem
            {
                LandfillItemSource = new List<LandfillItem>()
            };

            try
            {
                Connect();
                result = LoadHellperItemSource(customerType);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Dispose();
            }

            return result;
        }

    }
}
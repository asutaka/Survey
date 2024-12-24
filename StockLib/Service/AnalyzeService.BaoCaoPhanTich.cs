using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        public async Task<(int, string)> BaoCaoPhanTich(DateTime dt)
        {
            try
            {
                var sBuilder = new StringBuilder();
                var time = new DateTime(dt.Year, dt.Month, dt.Day);
                var d = int.Parse($"{time.Year}{time.Month.To2Digit()}{time.Day.To2Digit()}");

                var lDSC = await _apiService.DSC_GetPost();
                if(lDSC != null)
                {
                    var lValid = lDSC.Where(x => x.attributes.public_at > time);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.DSC),
                                builder.Eq(x => x.key, itemValid.id.ToString()),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.id.ToString(),
                                ty = (int)ESource.DSC
                            });

                            if (itemValid.attributes.category_id.data.attributes.slug.Equals("phan-tich-doanh-nghiep"))
                            {
                                var code = itemValid.attributes.slug.Split('-').First().ToUpper();
                                sBuilder.AppendLine($"[DSC - Phân tích cổ phiếu] - {code}:{itemValid.attributes.title}");
                                sBuilder.AppendLine($"Link: www.dsc.com.vn/bao-cao-phan-tich/{itemValid.attributes.slug}");
                            }
                            else if(!itemValid.attributes.category_id.data.attributes.slug.Contains("beat"))
                            {
                                sBuilder.AppendLine($"[DSC - Báo cáo phân tích]");
                                sBuilder.AppendLine($"Link: www.dsc.com.vn/bao-cao-phan-tich/{itemValid.attributes.slug}");
                            }
                        }
                    }
                }

                var lVNDirect = await _apiService.VNDirect_GetPost(false);
                if (lVNDirect != null)
                {
                    var t = int.Parse($"{time.Year}{time.Month.To2Digit()}{time.Day.To2Digit()}");
                    var lValid = lVNDirect.Where(x => int.Parse(x.newsDate.Replace("-","")) >= t);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.VNDirect),
                                builder.Eq(x => x.key, itemValid.newsId),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.newsId,
                                ty = (int)ESource.VNDirect
                            });

                            sBuilder.AppendLine($"[VNDirect - Phân tích cổ phiếu] {itemValid.newsTitle}");
                            if (itemValid.attachments.Any())
                            {
                                var link = $"https://www.vndirect.com.vn/cmsupload/beta/{itemValid.attachments.First().name}";
                            }
                            else
                            {
                                sBuilder.AppendLine($"Link: https://dstock.vndirect.com.vn/trung-tam-phan-tich/bao-cao-phan-tich-dn");
                            }
                        }
                    }
                }

                var lMigrateAsset = await _apiService.MigrateAsset_GetPost();
                if (lMigrateAsset != null)
                {
                    var lValid = lMigrateAsset.Where(x => x.published_at > time);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.MigrateAsset),
                                builder.Eq(x => x.key, itemValid.id.ToString()),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.id.ToString(),
                                ty = (int)ESource.MigrateAsset
                            });


                            if (itemValid.stock_related.Length == 3)
                            {
                                sBuilder.AppendLine($"[MigrateAsset - Phân tích cổ phiếu] {itemValid.stock_related}:{itemValid.title}");
                            }
                            else
                            {
                                sBuilder.AppendLine($"[MigrateAsset - Báo cáo phân tích] {itemValid.title}");
                            }
                            sBuilder.AppendLine($"Link: https://masvn.com/api{itemValid.file_path}");
                        }
                    }
                }

                var lAgribank = await _apiService.Agribank_GetPost(false);
                if (lAgribank != null)
                {
                    var lValid = lAgribank.Where(x => x.Date > time);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.Agribank),
                                builder.Eq(x => x.key, itemValid.ReportID.ToString()),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.ReportID.ToString(),
                                ty = (int)ESource.Agribank
                            });

                            if (itemValid.Title.Contains("AGR Snapshot"))
                            {
                                sBuilder.AppendLine($"[Agribank - Phân tích cổ phiếu] {itemValid.Title.Replace("AGR Snapshot", "").Trim()}");
                                sBuilder.AppendLine($"Link: https://agriseco.com.vn/Report/ReportFile/{itemValid.ReportID}");
                            }
                        }
                    }
                }

                var lVNDirectIndustry = await _apiService.VNDirect_GetPost(true);
                if (lVNDirectIndustry != null)
                {
                    var t = int.Parse($"{time.Year}{time.Month.To2Digit()}{time.Day.To2Digit()}");
                    var lValid = lVNDirect.Where(x => int.Parse(x.newsDate.Replace("-", "")) >= t);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.VNDirect),
                                builder.Eq(x => x.key, itemValid.newsId),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.newsId,
                                ty = (int)ESource.VNDirect
                            });

                            sBuilder.AppendLine($"[VNDirect - Báo cáo ngành] {itemValid.newsTitle}");
                            if (itemValid.attachments.Any())
                            {
                                var link = $"https://www.vndirect.com.vn/cmsupload/beta/{itemValid.attachments.First().name}";
                            }
                            else
                            {
                                sBuilder.AppendLine($"Link: https://dstock.vndirect.com.vn/trung-tam-phan-tich/bao-cao-nganh");
                            }
                        }
                    }
                }

                var lAgribankIndustry = await _apiService.Agribank_GetPost(true);
                if (lAgribankIndustry != null)
                {
                    var lValid = lAgribank.Where(x => x.Date > time);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.Agribank),
                                builder.Eq(x => x.key, itemValid.ReportID.ToString()),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.ReportID.ToString(),
                                ty = (int)ESource.Agribank
                            });

                            sBuilder.AppendLine($"[Agribank - Báo cáo ngành] {itemValid.Title.Trim()}");
                            sBuilder.AppendLine($"Link: https://agriseco.com.vn/Report/ReportFile/{itemValid.ReportID}");
                        }
                    }
                }

                //chưa pass
                var lSSI = await _apiService.SSI_GetPost();
                if (lSSI != null)
                {
                    var lValid = lSSI.Where(x => x.date >= time);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.SSI),
                                builder.Eq(x => x.key, itemValid.id),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.id,
                                ty = (int)ESource.SSI
                            });

                            sBuilder.AppendLine($"[SSI - Phân tích cổ phiếu] {itemValid.title}");
                            sBuilder.AppendLine($"Link: https://www.ssi.com.vn/khach-hang-ca-nhan/bao-cao-cong-ty");
                        }
                    }
                }

                var lVCI = await _apiService.VCI_GetPost();
                if (lVCI != null)
                {
                    var lValid = lVCI.Where(x => x.makerDate >= time
                                            && (x.pageLink == "company-research" || x.pageLink == "sector-reports" || x.pageLink == "macroeconomics" || x.pageLink == "phan-tich-doanh-nghiep"));
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.VCI),
                                builder.Eq(x => x.key, itemValid.id.ToString()),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.id.ToString(),
                                ty = (int)ESource.VCI
                            });

                            if (itemValid.pageLink == "company-research")
                            {
                                sBuilder.AppendLine($"[VCI - Phân tích cổ phiếu] {itemValid.name}");
                            }
                            else if (itemValid.pageLink == "sector-reports")
                            {
                                sBuilder.AppendLine($"[VCI - Báo cáo Ngành] {itemValid.name}");
                            }
                            else
                            {
                                sBuilder.AppendLine($"[VCI - Báo cáo vĩ mô] {itemValid.name}");
                            }

                            sBuilder.AppendLine($"Link: {itemValid.file}");
                        }
                    }
                }

                var lVCBS = await _apiService.VCBS_GetPost();
                if (lVCBS != null)
                {
                    var lValid = lVCBS.Where(x => x.publishedAt >= time
                                            && (x.category.code == "BCVM" || x.category.code == "BCDN" || x.category.code == "BCN"));
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.VCBS),
                                builder.Eq(x => x.key, itemValid.id.ToString()),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.id.ToString(),
                                ty = (int)ESource.VCBS
                            });

                            if(itemValid.category.code == "BCDN")
                            {
                                sBuilder.AppendLine($"[VCBS - Phân tích cổ phiếu] {itemValid.name}");
                            }
                            else if(itemValid.category.code == "BCN")
                            {
                                sBuilder.AppendLine($"[VCBS - Báo cáo Ngành] {itemValid.name}");
                            }
                            else
                            {
                                sBuilder.AppendLine($"[VCBS - Báo cáo vĩ mô] {itemValid.name}");
                            }
                            
                            sBuilder.AppendLine($"Link: https://www.vcbs.com.vn/trung-tam-phan-tich/bao-cao-chi-tiet");
                        }
                    }
                }

                var lBSC = await _apiService.BSC_GetPost();
                if (lBSC != null)
                {
                    var lValid = lBSC.Where(x => x.date >= time);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.BSC),
                                builder.Eq(x => x.key, itemValid.id),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.id,
                                ty = (int)ESource.BSC
                            });

                            sBuilder.AppendLine($"[BSC - Phân tích cổ phiếu] {itemValid.title}");
                            sBuilder.AppendLine($"Link: https://www.bsc.com.vn/bao-cao-phan-tich/danh-muc-bao-cao/1");
                        }
                    }
                }

                var lMBS = await _apiService.MBS_GetPost();
                if (lMBS != null)
                {
                    var lValid = lMBS.Where(x => x.date >= time);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.MBS),
                                builder.Eq(x => x.key, itemValid.id),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.id,
                                ty = (int)ESource.MBS
                            });

                            sBuilder.AppendLine($"[MBS - Phân tích cổ phiếu] {itemValid.title}");
                            sBuilder.AppendLine($"Link: https://mbs.com.vn/trung-tam-nghien-cuu/bao-cao-phan-tich/nghien-cuu-co-phieu/");
                        }
                    }
                }

                var lPSI = await _apiService.PSI_GetPost();
                if (lPSI != null)
                {
                    var lValid = lPSI.Where(x => x.date >= time);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.PSI),
                                builder.Eq(x => x.key, itemValid.id),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.id,
                                ty = (int)ESource.PSI
                            });

                            sBuilder.AppendLine($"[PSI - Phân tích cổ phiếu] {itemValid.title}");
                            sBuilder.AppendLine($"Link: https://www.psi.vn/vi/trung-tam-phan-tich/bao-cao-phan-tich-doanh-nghiep");
                        }
                    }
                }

                var lFPTS = await _apiService.FPTS_GetPost(false);
                if (lFPTS != null)
                {
                    var lValid = lFPTS.Where(x => x.date >= time);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.FPTS),
                                builder.Eq(x => x.key, itemValid.id),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.id,
                                ty = (int)ESource.FPTS
                            });

                            sBuilder.AppendLine($"[FPTS - Phân tích cổ phiếu] {itemValid.title}");
                            sBuilder.AppendLine($"Link: https://ezsearch.fpts.com.vn/Services/EzReport/?tabid=179");
                        }
                    }
                }

                var lFPTS_Nganh = await _apiService.FPTS_GetPost(true);
                if (lFPTS_Nganh != null)
                {
                    var lValid = lFPTS_Nganh.Where(x => x.date >= time);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.FPTS),
                                builder.Eq(x => x.key, itemValid.id),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.id,
                                ty = (int)ESource.FPTS
                            });

                            sBuilder.AppendLine($"[FPTS - Báo cáo Ngành] {itemValid.title}");
                            sBuilder.AppendLine($"Link: https://ezsearch.fpts.com.vn/Services/EzReport/?tabid=174");
                        }
                    }
                }

                var lCafeF = await _apiService.CafeF_GetPost();
                if (lCafeF != null)
                {
                    var lValid = lCafeF.Where(x => x.date >= time);
                    if (lValid?.Any() ?? false)
                    {
                        foreach (var itemValid in lValid)
                        {
                            FilterDefinition<ConfigBaoCaoPhanTich> filter = null;
                            var builder = Builders<ConfigBaoCaoPhanTich>.Filter;
                            var lFilter = new List<FilterDefinition<ConfigBaoCaoPhanTich>>()
                            {
                                builder.Eq(x => x.d, d),
                                builder.Eq(x => x.ty, (int)ESource.CafeF),
                                builder.Eq(x => x.key, itemValid.id),
                            };
                            foreach (var item in lFilter)
                            {
                                if (filter is null)
                                {
                                    filter = item;
                                    continue;
                                }
                                filter &= item;
                            }
                            var entityValid = _bcptRepo.GetEntityByFilter(filter);
                            if (entityValid != null)
                                continue;

                            _bcptRepo.InsertOne(new ConfigBaoCaoPhanTich
                            {
                                d = d,
                                key = itemValid.id,
                                ty = (int)ESource.CafeF
                            });

                            sBuilder.AppendLine($"[CafeF - Phân tích] {itemValid.title}");
                            sBuilder.AppendLine($"Link: https://s.cafef.vn/phan-tich-bao-cao.chn");
                        }
                    }
                }

                if (sBuilder.Length > 0)
                {
                    return (1, sBuilder.ToString());
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError($"AnalyzeService.BaoCaoPhanTich|EXCEPTION| {ex.Message}");
            }
            return (0, null);
        }
    }
}

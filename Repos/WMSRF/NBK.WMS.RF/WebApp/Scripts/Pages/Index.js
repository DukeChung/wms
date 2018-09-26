
//切换仓库
function GetWareHouse(id, name) {
    SetlocalStorage("WarehouseSysId", id);
    SetlocalStorage("WarehouseName", name);
    $("#wareHouseName").html(name);
}

//退出登录
function ExitLogin() {
    if (confirm("确定要退出登录?")) {
        RemovelocalStorage("UserName");
        RemovelocalStorage("DisplayName");
        RemovelocalStorage("UserId");
        RemovelocalStorage("WarehouseSysId");
        RemovelocalStorage("WarehouseName");
        RemovelocalStorage("WarehouseList");
        $(".warehouselist").html("");
        $.router.load("Login.html");
    }
}

//Login页面
$(document).on("pageInit", "#page-login", function (e, pageId, $page) {
    //ssoLogin();

    $("#username").focus();
    if (GetlocalStorage("UserId") != "") {
        $.router.load("Home.html");
    }

    $("#username").keydown(function () {
        if (event.keyCode == 13) {
            if ($("#username").val() != "") {
                $("#password").focus();
            } else {
                $("#username").focus();
            }
        }
    });

    $("#password").keydown(function () {
        if (event.keyCode == 13) {
            login();
        }
    });
});

//Home主页
$(document).on("pageInit", "#page-home-index", function (e, pageId, $page) {
    sku.OnClose();

    if (GetlocalStorage("UserId") == null || GetlocalStorage("UserId") == undefined || GetlocalStorage("UserId") == "") {
        $.router.load("Login.html");
    }

    $("#wareHouseName").html(GetlocalStorage("WarehouseName"));

    if ($(".warehouselist").html() == "") {
        var wareHouses = JSON.parse(GetlocalStorage("WarehouseList"));
        var strs = new Array();
        for (var i = 0; i < wareHouses.length; i++) {
            strs.push('<div class="pad-t-b-5 close-panel" onclick="GetWareHouse(\'' + wareHouses[i].SysId + '\', \'' + wareHouses[i].Name + '\');">' + wareHouses[i].Name + '</div>');
        }
        $(".warehouselist").html(strs.join(''));
    }
});

//待上架收货单据列表
$(document).on("pageInit", "#page-shelves-index", function (e, pageId, $page) {
    $("#order").focus();
    $("#msg").html("");
    $("#ShelvesIndexList").html("");

    $("#order").keydown(function () {
        if (event.keyCode == 13) {
            var receiptOrder = $("#order").val();

            com.CommonNoLoadingAjax("api/Shelves/CheckReceiptOrder", { ReceiptOrderSearch: receiptOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
                if (data.IsSucess) {
                    $("#msg").html("");
                    if (receiptOrder.trim() != "") {
                        $.router.load("ShelvesListing.html?receiptOrder=" + receiptOrder);
                    }
                } else {
                    com.FailMsg("#msg", data.Message);
                    $("#order").focus();
                    $("#order").val("");
                }
            });
        }
    });

    function WaitingShelvesList() {
        loading = true;
        com.CommonLoadDataAjax("api/Shelves/GetWaitingShelvesList",
            {
                WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                iDisplayStart: iDisplayStart,
                iDisplayLength: iDisplayLength
            },
            function (data) {
                if (data.TableResuls.aaData != null && data.TableResuls.aaData.length > 0) {
                    var jsonArray = new Array();
                    for (var i = 0; i < data.TableResuls.aaData.length; i++) {
                        jsonArray.push('<tr onclick="$.router.load(\'ShelvesListing.html?receiptOrder=' + data.TableResuls.aaData[i].ReceiptOrder + '\');">');
                        jsonArray.push('<td>' + (iDisplayStart + (i + 1)) + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].ReceiptOrder + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].SkuNumber + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].DisplaySkuQty + '</td></tr> ');
                    }
                    $("#ShelvesIndexList").append(jsonArray.join(''));
                    iTotalDisplayRecords = data.TableResuls.iTotalDisplayRecords;
                    loading = false;

                    com.NoScroll();
                } else {
                    if (data.TableResuls.iTotalDisplayRecords == 0) {
                        com.NoData("#ShelvesIndexList");
                    }
                }
            });
    }

    com.ScrollLoadData(WaitingShelvesList);
    WaitingShelvesList();
});

//上架清单
$(document).on("pageInit", "#page-shelves-listing", function (e, pageId, $page) {
    sku.OnClose();

    $("#ShelvesListing").html("");
    var receiptOrder = com.GetQueryString("receiptOrder");

    com.CommonAjax("api/Shelves/GetWaitingShelvesSkuList", { ReceiptOrder: receiptOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
        sku.SetSkuDetailList(data);
        if (data != null && data.length > 0) {
            var jsonArray = new Array();
            var totalQty = 0;
            var totalSkuQty = 0;
            for (var i = 0; i < data.length; i++) {
                jsonArray.push('<tr>');
                jsonArray.push('<td>' + (i + 1) + '</td>');
                jsonArray.push('<td>' + data[i].UPC + '</td>');
                jsonArray.push('<td>' + data[i].SkuName + '</td>');
                jsonArray.push('<td>' + data[i].DisplaySkuQty + '</td>');
                totalQty += data[i].DisplayReceivedQty;
                totalSkuQty += data[i].DisplaySkuQty;
            }
            var progress = ((totalQty - totalSkuQty) / totalQty) * 100;
            $("#progress").html(parseInt(progress))
            $("#ShelvesListing").html(jsonArray.join(''));
        }
    });

    $(".btn").on("click", function () {
        if ($("#progress").html() == "100") {
            alert("上架已完成");
        } else {
            $.router.load("ShelvesDetail.html?receiptOrder=" + receiptOrder);
        }
    });
});

//扫描上架
$(document).on("pageInit", "#page-shelves-detail", function (e, pageId, $page) {
    var receiptOrder = com.GetQueryString("receiptOrder");

    $("#skuUPC").focus();
    $("#msg").html("");
    $("#skuList").html("");

    //UPC
    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#loc").focus();
        }
    });
    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            if (!sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#loc", null)) {
                com.FailMsg("#msg", "扫描条码在此单据不存在");
                $("#skuUPC").focus();
                $("#skuUPC").val("");
                return false;
            } else {
                $("#msg").html("");
                $("#loc").focus();
            }

            //获取推荐货位
            com.CommonAjax("api/Shelves/GetAdviceToLoc",
                        {
                            ReceiptOrder: receiptOrder,
                            SkuSysId: sku.GetSkuSysId(),
                            UPC: $("#skuUPC").val(),
                            WarehouseSysId: GetlocalStorage("WarehouseSysId")
                        },
                        function (data) {
                            if (data != null) {
                                if ($("#loc").val() == "") {
                                    $("#loc").val(data);
                                    $("#loc").focus();
                                }
                            }
                        });

            //查询库存
            com.CommonAjax("api/Shelves/GetInventoryList",
                        {
                            ReceiptOrder: receiptOrder,
                            SkuSysId: sku.GetSkuSysId(),
                            UPC: $("#skuUPC").val(),
                            WarehouseSysId: GetlocalStorage("WarehouseSysId")
                        },
                        function (data) {
                            if (data != null && data.length > 0) {
                                var jsonArray = new Array();
                                jsonArray.push('提示：当前商品在以下库位中有库存');
                                for (var i = 0; i < data.length; i++) {
                                    jsonArray.push('<div class="row">');
                                    jsonArray.push('<div class="col-xs-5">' + data[i].Loc + '</div>');
                                    jsonArray.push('<div class="col-xs-7">' + data[i].DisplayQty + '</div>');
                                    jsonArray.push('</div>');
                                }
                                $("#skuList").html(jsonArray.join(''));
                            }
                        });

            //com.CommonAjax("api/Shelves/CheckReceiptDetailSku",
            //    {
            //        ReceiptOrder: receiptOrder,
            //        UPC: $("#skuUPC").val(),
            //        WarehouseSysId: GetlocalStorage("WarehouseSysId")
            //    },
            //    function (data) {
            //        if (data != null) {
            //            if (!data.IsSucess) {
            //                com.FailMsg("#msg", data.Message);
            //                $("#skuUPC").focus();
            //                $("#skuUPC").val("");
            //            } else {
            //                $("#msg").html("");
            //                $("#loc").focus();

            //                //获取推荐货位
            //                com.CommonAjax("api/Shelves/GetAdviceToLoc",
            //                            {
            //                                ReceiptOrder: receiptOrder,
            //                                UPC: $("#skuUPC").val(),
            //                                WarehouseSysId: GetlocalStorage("WarehouseSysId")
            //                            },
            //                            function (data) {
            //                                if (data != null) {
            //                                    if ($("#loc").val() == "") {
            //                                        $("#loc").val(data);
            //                                        $("#loc").focus();
            //                                    }
            //                                }
            //                            });

            //                //查询库存
            //                com.CommonAjax("api/Shelves/GetInventoryList",
            //                            {
            //                                ReceiptOrder: receiptOrder,
            //                                UPC: $("#skuUPC").val(),
            //                                WarehouseSysId: GetlocalStorage("WarehouseSysId")
            //                            },
            //                            function (data) {
            //                                if (data != null && data.length > 0) {
            //                                    var jsonArray = new Array();
            //                                    jsonArray.push('提示：当前商品在以下库位中有库存');
            //                                    for (var i = 0; i < data.length; i++) {
            //                                        jsonArray.push('<div class="row">');
            //                                        jsonArray.push('<div class="col-xs-5">' + data[i].Loc + '</div>');
            //                                        jsonArray.push('<div class="col-xs-7">' + data[i].DisplayQty + '</div>');
            //                                        jsonArray.push('</div>');
            //                                    }
            //                                    $("#skuList").html(jsonArray.join(''));
            //                                }
            //                            });
            //            }
            //        }
            //    });
        }
    });

    //货位
    $("#loc").on("keydown", function () {
        if (event.keyCode == 13) {
            $("#lot").focus();
        }
    });

    $("#loc").on("blur", function () {
        if ($("#loc").val().trim() != "") {
            com.CommonAjax("api/Base/LocIsExist",
                {
                    LocationSearch: $("#loc").val(),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    if (data != null) {
                        if (!data.IsSucess) {
                            com.FailMsg("#msg", data.Message);
                            $("#loc").focus();
                            $("#loc").val("");
                        } else {
                            $("#msg").html("");
                        }
                    }
                });
        }
    });

    //批次
    $("#lot").on("keydown", function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });

    //上架数量必须为数字
    $("#qty").on("keyup", function () {
        if (!com.CheckNumber($("#qty").val())) {
            $("#qty").val("");
        }
    });

    $("#qty").on("keydown", function () {
        $("#msg").html("");
        if (event.keyCode == 13) {
            var loc = $("#loc").val();
            var lot = $("#lot").val();
            var upc = $("#skuUPC").val();
            var qty = $("#qty").val();
            var skuSysId = sku.GetSkuSysId();

            if (upc.trim() == "") {
                com.FailMsg("#msg", "商品条码不能为空");
                $("#skuUPC").focus();
                return false;
            }
            if (loc.trim() == "") {
                com.FailMsg("#msg", "货位不能为空");
                $("#loc").focus();
                return false;
            }
            if (qty.trim() == "") {
                com.FailMsg("#msg", "商品数量不能为空");
                $("#qty").focus();
                return false;
            }
            if (parseInt(qty.trim()) <= 0) {
                com.FailMsg("#msg", "商品数量必须大于0");
                $("#qty").focus();
                return false;
            }
            $.showIndicator();
            com.CommonAjax("api/Shelves/ScanShelves",
                {
                    ReceiptOrder: receiptOrder,
                    SkuSysId: skuSysId,
                    UPC: upc,
                    Loc: loc,
                    Lot: lot,
                    InputQty: qty,
                    UserId: GetlocalStorage("UserId"),
                    CurrentDisplayName: GetlocalStorage("DisplayName"),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    $.hideIndicator();
                    $("#skuList").html("");
                    if (data != null) {
                        if (data.IsSucess) {
                            com.SuccessMsg("#msg", data.Message);
                            $("#skuUPC").val("");
                            $("#loc").val("");
                            $("#lot").val("");
                            $("#qty").val("");
                            $("#skuUPC").focus();
                        } else {
                            com.FailMsg("#msg", data.Message);
                        }
                    }
                });
        }
    });
});

//初盘
$(document).on("pageInit", "#page-inventory-first", function (e, pageId, $page) {
    sku.OnClose();
    $("#order").focus();
    $("#msg").html("");
    $("#firstInventoryList").html("");

    $("#order").keydown(function () {
        if (event.keyCode == 13) {
            var stockTakeOrder = $("#order").val();
            com.CommonNoLoadingAjax("api/RFStockTake/CheckFirstStockTake", {
                WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                CurrentUserId: GetlocalStorage("UserId"),
                StockTakeOrder: stockTakeOrder
            }, function (data) {
                if (data.IsSucess) {
                    $("#msg").html("");
                    if (stockTakeOrder.trim() != "") {
                        $.router.load("FirstInventoryDetail.html?stockTakeOrder=" + stockTakeOrder);
                    }
                } else {
                    com.FailMsg("#msg", data.Message);
                    $("#order").val("");
                    $("#order").focus();
                }
            });
        }
    });

    function getStockTakeFirstList() {
        loading = true;
        com.CommonLoadDataAjax("api/RFStockTake/GetStockTakeFirstListByPaging", {
            CurrentUserId: GetlocalStorage("UserId"),
            WarehouseSysId: GetlocalStorage("WarehouseSysId"),
            iDisplayStart: iDisplayStart,
            iDisplayLength: iDisplayLength
        }, function (data) {
            if (data.TableResuls.aaData != null && data.TableResuls.aaData.length > 0) {
                var jsonArray = new Array();
                for (var i = 0; i < data.TableResuls.aaData.length; i++) {
                    jsonArray.push('<tr onclick="$.router.load(\'FirstInventoryDetail.html?stockTakeOrder=' + data.TableResuls.aaData[i].StockTakeOrder + '\');">');
                    jsonArray.push('<td>' + (iDisplayStart + (i + 1)) + '</td>');
                    jsonArray.push('<td>' + data.TableResuls.aaData[i].StockTakeOrder + '</td>');
                    jsonArray.push('<td>' + data.TableResuls.aaData[i].SkuCount + '</td>');
                    jsonArray.push('<td>' + data.TableResuls.aaData[i].StatusName + '</td></tr> ');
                }
                $("#firstInventoryList").append(jsonArray.join(''));
                iTotalDisplayRecords = data.TableResuls.iTotalDisplayRecords;
                loading = false;
                com.NoScroll();
            } else {
                if (data.TableResuls.iTotalDisplayRecords == 0) {
                    com.NoData("#firstInventoryList");
                }
            }
        });
    }

    com.ScrollLoadData(getStockTakeFirstList);
    getStockTakeFirstList();
});

//初盘明细
$(document).on("pageInit", "#page-inventory-first-detail", function (e, pageId, $page) {
    $("#firstInventoryDetailList").html("");
    $("#msg").html("");
    $("#skuCount").html("0");
    $("#loc").val("");
    $("#qty").val("");
    $("#skuUPC").val("");
    $("#skuUPC").focus();
    var stockTakeOrder = com.GetQueryString("stockTakeOrder");
    var scanSkus = new Array();

    getStockTakeFirstDetailList();

    //获取初盘明细
    function getStockTakeFirstDetailList() {
        com.CommonAjax("api/RFStockTake/GetStockTakeFirstDetailList", {
            StockTakeOrder: stockTakeOrder,
            WarehouseSysId: GetlocalStorage("WarehouseSysId"),
            CurrentUserId: GetlocalStorage("UserId")
        }, function (data) {
            if (data != null && data.length > 0) {
                var distinctData = [];
                //sku.SetSkuDetailList(data);
                var jsonArray = new Array();
                for (var i = 0; i < data.length; i++) {
                    var isDistinct = false;
                    if (distinctData.length > 0) {
                        for (var j = 0; j < distinctData.length; j++) {
                            dData = distinctData[j];
                            if (dData.SkuSysId == data[i].SkuSysId) {
                                isDistinct = true;
                                break;
                            }
                        }
                    }
                    if (isDistinct == false) {
                        distinctData.push(data[i]);
                    }

                    jsonArray.push('<tr>');
                    jsonArray.push('<td>' + (i + 1) + '</td>');
                    jsonArray.push('<td>' + data[i].UPC + '</td>');
                    jsonArray.push('<td>' + data[i].SkuName + '</td>');
                    jsonArray.push('<td>' + data[i].Loc + '</td>');
                    jsonArray.push('<td>' + data[i].DisplayStockTakeQty + '</td></tr>');
                }
                sku.SetSkuDetailList(distinctData);
                $("#firstInventoryDetailList").append(jsonArray.join(''));
            } else {
                com.NoData("#firstInventoryDetailList");
            }
        });
    }

    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#loc").focus();
        }
    });

    $("#loc").keydown(function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });

    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            if (!sku.OnPicker($("#skuUPC").val().trim(), "#loc", "#loc", null)) {
                com.FailMsg("#msg", "扫描条码在此单据不存在");
                $("#skuUPC").val("");
                $("#skuUPC").focus();
                return false;
            }
            $("#msg").html("");
            $("#loc").focus();
        }
    });

    $("#qty").on("keyup", function () {
        if (!com.CheckNumber($("#qty").val())) {
            $("#qty").val("");
        }
    });

    $("#qty").on("keydown", function () {
        $("#msg").html("");
        if (event.keyCode == 13) {
            var skuUPC = $("#skuUPC").val();
            var loc = $("#loc").val().trim();
            var qty = $("#qty").val();

            if (skuUPC.trim() == "") {
                com.FailMsg("#msg", "商品条码不能为空");
                $("#skuUPC").focus();
                return false;
            }
            if (loc == "") {
                com.FailMsg("#msg", "库位不能为空");
                $("#loc").focus();
                return false;
            }
            if (qty.trim() == "") {
                com.FailMsg("#msg", "商品数量不能为空");
                $("#qty").focus();
                return false;
            }

            $.showIndicator();
            com.CommonNoLoadingAjax("api/RFStockTake/StockTakeFirstScanning", {
                StockTakeOrder: stockTakeOrder,
                SkuSysId: sku.GetSkuSysId(),
                UPC: skuUPC,
                Loc: loc,
                InputQty: qty,
                CurrentUserId: GetlocalStorage("UserId"),
                CurrentDisplayName: GetlocalStorage("DisplayName"),
                WarehouseSysId: GetlocalStorage("WarehouseSysId")
            }, function (data) {
                $.hideIndicator();
                if (data != null) {
                    if (data.IsSucess) {
                        $("#skuUPC").val("");
                        $("#loc").val("");
                        $("#qty").val("");
                        $("#skuUPC").focus();
                        $("#firstInventoryDetailList").html("");
                        var upcFlag = $("#selectkey").val().split('|')[0];
                        if (upcFlag == "S") {
                            scanSkus.push($("#selectkey").val());
                            $("#skuCount").text(getCount(scanSkus));
                        }
                        getStockTakeFirstDetailList();
                        com.SuccessMsg("#msg", data.Message);
                    } else {
                        com.FailMsg("#msg", data.Message);
                    }
                }
            });
        }
    });

    $("#btnFinish").on("click", function () {
        $.showIndicator();
        com.CommonNoLoadingAjax("api/RFStockTake/UpdateFirstStockTakeStatus", {
            StockTakeOrder: stockTakeOrder,
            CurrentUserId: GetlocalStorage("UserId"),
            CurrentDisplayName: GetlocalStorage("DisplayName"),
            WarehouseSysId: GetlocalStorage("WarehouseSysId")
        }, function (data) {
            $.hideIndicator();
            if (data != null) {
                if (data.IsSucess) {
                    $.router.load("../../Home.html");
                } else {
                    com.FailMsg("#msg", data.Message);
                }
            }
        });
    });

    $("#txt_DetailList").on("click", function () {
        if ($("#icon_DetailList").hasClass("fa fa-chevron-down")) {
            $("#icon_DetailList").attr("class", "fa fa-chevron-up");
            $("#div_DetailList").show("normal");
        } else {
            $("#icon_DetailList").attr("class", "fa fa-chevron-down");
            $("#div_DetailList").hide("normal");
        }
    });

    function getCount(arr) {
        var n = [];
        for (var i = 0; i < arr.length; i++) {
            if (n.indexOf(arr[i]) == -1) n.push(arr[i]);
        }
        return n.length;
    }
});

//复盘单据列表
$(document).on("pageInit", "#page-inventory-second", function (e, pageId, $page) {
    sku.OnClose();
    $("#order").focus();
    $("#msg").html("");
    $("#SecondIndexList").html("");

    $("#order").keydown(function () {
        if (event.keyCode == 13) {
            var stockTakeOrder = $("#order").val();

            com.CommonNoLoadingAjax("api/RFStockTake/CheckSecondStockTake",
                {
                    StockTakeOrder: stockTakeOrder,
                    WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                    CurrentUserId: GetlocalStorage("UserId")
                },
                function (data) {
                    if (data.IsSucess) {
                        $("#msg").html("");
                        if (stockTakeOrder.trim() != "") {
                            $.router.load("SecondInventoryDetail.html?stockTakeOrder=" + stockTakeOrder);
                        }
                    } else {
                        com.FailMsg("#msg", data.Message);
                        $("#order").focus();
                        $("#order").val("");
                    }
                });
        }
    });

    function SecondInventoryList() {
        loading = true;
        com.CommonLoadDataAjax("api/RFStockTake/GetStockTakeSecondListByPage",
            {
                WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                iDisplayStart: iDisplayStart,
                iDisplayLength: iDisplayLength,
                CurrentUserId: GetlocalStorage("UserId")
            },
            function (data) {
                if (data.TableResuls.aaData != null && data.TableResuls.aaData.length > 0) {
                    var jsonArray = new Array();
                    for (var i = 0; i < data.TableResuls.aaData.length; i++) {
                        jsonArray.push('<tr onclick="$.router.load(\'SecondInventoryDetail.html?stockTakeOrder=' + data.TableResuls.aaData[i].StockTakeOrder + '\');">');
                        jsonArray.push('<td>' + (iDisplayStart + (i + 1)) + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].StockTakeOrder + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].SkuCount + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].StatusName + '</td></tr> ');
                    }
                    $("#SecondIndexList").append(jsonArray.join(''));
                    iTotalDisplayRecords = data.TableResuls.iTotalDisplayRecords;
                    loading = false;

                    com.NoScroll();
                } else {
                    if (data.TableResuls.iTotalDisplayRecords == 0) {
                        com.NoData("#SecondIndexList");
                    }
                }
            });
    }

    com.ScrollLoadData(SecondInventoryList);
    SecondInventoryList();
});

//复盘
$(document).on("pageInit", "#page-inventory-detail", function (e, pageId, $page) {
    var stockTakeOrder = com.GetQueryString("stockTakeOrder");

    $("#StockTakeSecondDetailList").html("");
    $("#detailList").hide();
    $("#icon_DetailList").removeClass("fa-chevron-up").addClass("fa-chevron-down");
    $("#skuCount").html(0);
    $("#skuUPC").val("");
    $("#skuUPC").focus();
    $("#loc").val("");
    $("#qty").val("");
    $("#msg").html("");
    var scanSkus = new Array();

    StockTakeSecondDetailList();

    //复盘明细
    function StockTakeSecondDetailList() {
        com.CommonAjax("api/RFStockTake/GetStockTakeSecondList",
            {
                StockTakeOrder: stockTakeOrder,
                WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                CurrentUserId: GetlocalStorage("UserId")
            },
            function (data) {
                //sku.SetSkuDetailList(data);
                if (data != null && data.length > 0) {
                    var distinctData = [];
                    var jsonArray = new Array();
                    for (var i = 0; i < data.length; i++) {
                        var isDistinct = false;
                        if (distinctData.length > 0) {
                            for (var j = 0; j < distinctData.length; j++) {
                                dData = distinctData[j];
                                if (dData.SkuSysId == data[i].SkuSysId) {
                                    isDistinct = true;
                                    break;
                                }
                            }
                        }
                        if (isDistinct == false) {
                            distinctData.push(data[i]);
                        }

                        jsonArray.push('<tr>');
                        jsonArray.push('<td>' + (i + 1) + '</td>');
                        jsonArray.push('<td>' + data[i].UPC + '</td>');
                        jsonArray.push('<td>' + data[i].SkuName + '</td>');
                        jsonArray.push('<td>' + data[i].Loc + '</td>');
                        jsonArray.push('<td>' + ((data[i].DisplayReplayQty == null || data[i].DisplayReplayQty.undefined) ? "0" : data[i].DisplayReplayQty) + '</td>');
                    }
                    sku.SetSkuDetailList(distinctData);
                    $("#StockTakeSecondDetailList").append(jsonArray.join(''));
                    //$("#skuCount").html(data.length);
                } else {
                    com.NoData("#StockTakeSecondDetailList");
                }
            });
    }

    //UPC
    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#loc").focus();
        }
    });

    $("#loc").keydown(function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });

    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            if (!sku.OnPicker($("#skuUPC").val().trim(), "#loc", "#loc", null)) {
                com.FailMsg("#msg", "扫描条码在此单据不存在");
                $("#skuUPC").focus();
                $("#skuUPC").val("");
                return false;
            } else {
                $("#msg").html("");
                $("#loc").focus();
            }
        }
    });

    //商品数量必须为数字
    $("#qty").on("keyup", function () {
        if (!com.CheckNumber($("#qty").val())) {
            $("#qty").val("");
        }
    });
    $("#qty").on("keydown", function () {
        if (event.keyCode == 13) {
            var upc = $("#skuUPC").val();
            var loc = $("#loc").val().trim();
            var qty = $("#qty").val();

            if (upc.trim() == "") {
                com.FailMsg("#msg", "商品条码不能为空");
                $("#skuUPC").focus();
                return false;
            }
            if (loc == "") {
                com.FailMsg("#msg", "库位不能为空");
                $("#loc").focus();
                return false;
            }
            if (qty.trim() == "") {
                com.FailMsg("#msg", "商品数量不能为空");
                $("#qty").focus();
                return false;
            }

            $.showIndicator();
            com.CommonNoLoadingAjax("api/RFStockTake/StockTakeSecond",
                {
                    StockTakeOrder: stockTakeOrder,
                    SkuSysId: sku.GetSkuSysId(),
                    UPC: upc,
                    Loc: loc,
                    InputQty: qty,
                    CurrentUserId: GetlocalStorage("UserId"),
                    CurrentDisplayName: GetlocalStorage("DisplayName"),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    $.hideIndicator();
                    if (data != null) {
                        if (data.IsSucess) {
                            $("#skuUPC").val("");
                            $("#skuUPC").focus();
                            $("#loc").val("");
                            $("#qty").val("");
                            $("#StockTakeSecondDetailList").html("");
                            var upcFlag = $("#selectkey").val().split('|')[0];
                            if (upcFlag == "S") {
                                scanSkus.push($("#selectkey").val());
                                $("#skuCount").text(getCount(scanSkus));
                            }
                            StockTakeSecondDetailList();
                            com.SuccessMsg("#msg", "UPC：" + upc + "，数量：" + qty + "，盘点成功！");
                        } else {
                            com.FailMsg("#msg", data.Message);
                        }
                    }
                });
        }
    });

    //复盘提交
    $("#btnSubmit").on("click", function () {
        $.showIndicator();
        com.CommonNoLoadingAjax("api/RFStockTake/UpdateSecondStockTakeStatus",
            {
                StockTakeOrder: stockTakeOrder,
                CurrentUserId: GetlocalStorage("UserId"),
                CurrentDisplayName: GetlocalStorage("DisplayName"),
                WarehouseSysId: GetlocalStorage("WarehouseSysId")
            },
            function (data) {
                $.hideIndicator();
                if (data != null) {
                    if (data.IsSucess) {
                        $.router.load("../../Home.html");
                    } else {
                        com.FailMsg("#msg", data.Message);
                    }
                }
            });
    });

    $("#txt_DetailList").on("click", function () {
        if ($j("#detailList").is(":hidden")) {
            $("#detailList").show("normal");
            $("#icon_DetailList").removeClass("fa-chevron-down").addClass("fa-chevron-up");
        } else {
            $("#detailList").hide("normal");
            $("#icon_DetailList").removeClass("fa-chevron-up").addClass("fa-chevron-down");
        }
    });

    function getCount(arr) {
        var n = [];
        for (var i = 0; i < arr.length; i++) {
            if (n.indexOf(arr[i]) == -1) n.push(arr[i]);
        }
        return n.length;
    }
});

//扫描箱号发货
$(document).on("pageInit", "#page-ScanVanning-listing", function (e, pageId, $page) {
    $("#VanningSysId").val("");
    $("#msg").html("");
    $("#divMessage").html("");
    $("#divVanningBox").html("");
    $("#VanningOrder").focus();
    var numberList = "";
    var isLoad = false;
    var type = "VanningOrder";

    $("#VanningOrder").on("keydown", function () {
        if (event.keyCode == 13) {
            $("#msg").html("");
            var number = $("#VanningOrder").val();
            //$("#divVanningBox").html("");
            $("#VanningOrder").val("");
            if (number.length === 0) {
                return;
            }
            if ($("#div" + number).length !== 0) {
                $("#div" + number).removeClass("bg-muted");
                $("#div" + number).addClass("navy-bg");
                CheckMsg(type);

            } else {
                var numberKey = number.split('-');
                if (numberList.length > 0) {
                    var list = numberList.split(',');

                    if (list.length > 0) {
                        for (var i = 0; list.length > i; i++) {
                            if (list[i] === numberKey[0]) {
                                com.FailMsg("#msg", "未找到相关信息!");
                                return;
                            }
                        }
                    }
                }
                numberList += numberKey[0] + ",";

                com.CommonAjax("api/RFOutbound/GetDeliveryBoxByOrderNumber?type=" + "VanningOrder" + "&orderNumber=" + number + "&wareHouseSysId=" + GetlocalStorage("WarehouseSysId"),
                {},
                function (data) {
                    if (data != null) {

                        if (data.length > 0) {
                            $("#VanningSysId").val($("#VanningSysId").val() + ',' + data[0].VanningSysId);
                            isLoad = true;
                            for (var i = 0; data.length > i; i++) {

                                var html = $("#divBoxTemp").html();
                                var vanningOrder = data[i].VanningOrder + "-" + data[i].ContainerNumber;
                                var className = "";
                                if (vanningOrder === number || data[i].CarrierNumber === number) {
                                    className = "navy-bg";
                                } else {
                                    className = "bg-muted";
                                }
                                html = html
                                    .replace(/CarrierName/g, data[i].CarrierName == undefined ? "" : data[i].CarrierName)
                                    .replace(/Weight/g, data[i].Weight)
                                    .replace(/className/g, className);

                                html = html.replace(/VanningOrder/g, vanningOrder)
                                        .replace(/divVanningOrder/g, "div" + vanningOrder);
                                $("#divVanningBox").append(html);
                            }
                            $("#btnDelivery").attr("disabled", "disabled");
                            CheckMsg("VanningOrder");
                        } else {
                            com.FailMsg("#msg", "未找到相关信息!");
                        }
                    }
                });
            }
        }
    });

    function CheckMsg(type) {
        var html = '<div class="alert className"> Msg</div>';

        if ($(".bg-muted").length > 0) {

            var msg = "当前还剩余" + $(".bg-muted").length + "箱未扫描";
            html = html.replace(/Msg/g, msg)
                .replace(/className/g, "alert-warning");

        } else {
            msg = "当前已经全部扫描,可以进行发货";
            html = html.replace(/Msg/g, msg)
                .replace(/className/g, "alert-success");
            $('#btnDelivery').removeAttr("disabled");
        }
        if (type === "VanningOrder") {

            $("#divMessage").html(html);
        } else {
            $("#divMessageCarrierNumber").html(html);
        }
    }

    $("#btnDelivery").bind("click", function () {
        if ($("#btnDelivery").attr("disabled") === "disabled") {
            return;
        }

        $.showIndicator();
        var sysIds = $("#VanningSysId").val();
        if (sysIds.length > 0 && sysIds[0] == ',') {
            sysIds = sysIds.substring(1);
        }
        com.CommonAjax("api/RFOutbound/SaveDeliveryByVanningSysId",
            {
                vanningSysIds: sysIds.split(','),
                currentUserName: GetlocalStorage("DisplayName"),
                currentUserId: GetlocalStorage("UserId"),
                wareHouseSysId: GetlocalStorage("WarehouseSysId")
            },
            function (data) {
                $.hideIndicator();
                if (data != null) {
                    if (data.Message == undefined) {
                        com.SuccessMsg("#msg", "发货完成");
                        $("#VanningSysId").val("");
                        $("#divMessage").html("");
                        $("#divVanningBox").html("");
                        $("#VanningOrder").focus();
                        numberList = "";
                    }
                    else {
                        com.FailMsg("#msg", data.Message);
                    }
                }
            });


    });
});

//扫描快递单号发货
$(document).on("pageInit", "#page-ScanExpressOrder-listing", function (e, pageId, $page) {
    $("#msg").html("");
    $("#divMessageCarrierNumber").html("");
    $("#divCarrierBox").html("");
    $("#CarrierNumber").focus();
    var numberList = "";
    var isLoad = false;
    var type = "CarrierNumber";

    $("#CarrierNumber").on("keydown", function () {
        if (event.keyCode == 13) {
            $("#msg").html("");
            var number = $("#CarrierNumber").val();
            //$("#divCarrierBox").html("");
            $("#CarrierNumber").val("");
            if (number.length === 0) {
                return;
            }
            if ($("#div" + number).length !== 0) {
                $("#div" + number).removeClass("bg-muted");
                $("#div" + number).addClass("navy-bg");
                CheckMsg(type);

            } else {
                var numberKey = number.split('-');
                if (numberList.length > 0) {
                    var list = numberList.split(',');

                    if (list.length > 0) {
                        for (var i = 0; list.length > i; i++) {
                            if (list[i] === numberKey[0]) {
                                com.FailMsg("#msg", "未找到相关信息!");
                                return;
                            }
                        }
                    }
                }
                numberList += numberKey[0] + ",";

                com.CommonAjax("api/RFOutbound/GetDeliveryBoxByOrderNumber?type=" + type + "&orderNumber=" + number + "&wareHouseSysId=" + GetlocalStorage("WarehouseSysId"),
                {},
                function (data) {
                    if (data != null) {

                        if (data.length > 0) {
                            $("#VanningSysId").val($("#VanningSysId").val() + ',' + data[0].VanningSysId);
                            isLoad = true;
                            for (var i = 0; data.length > i; i++) {

                                var html = $("#divBoxTemp").html();
                                var vanningOrder = data[i].VanningOrder + "-" + data[i].ContainerNumber;
                                var className = "";
                                if (vanningOrder === number || data[i].CarrierNumber === number) {
                                    className = "navy-bg";
                                } else {
                                    className = "bg-muted";
                                }
                                html = html
                                    .replace(/CarrierName/g, data[i].CarrierName)
                                    .replace(/Weight/g, data[i].Weight)
                                    .replace(/className/g, className);

                                html = html.replace(/VanningOrder/g, data[i].CarrierNumber)
                                        .replace(/divVanningOrder/g, "div" + data[i].CarrierNumber);
                                $("#divCarrierBox").append(html);
                            }
                            $("#btnDelivery").attr("disabled", "disabled");
                            CheckMsg(type);
                        } else {
                            com.FailMsg("#msg", "未找到相关信息!");
                        }
                    }
                });
            }
        }
    });

    function CheckMsg(type) {
        var html = '<div class="alert className"> Msg</div>';

        if ($(".bg-muted").length > 0) {

            var msg = "当前还剩余" + $(".bg-muted").length + "箱未扫描";
            html = html.replace(/Msg/g, msg)
                .replace(/className/g, "alert-warning");

        } else {
            msg = "当前已经全部扫描,可以进行发货";
            html = html.replace(/Msg/g, msg)
                .replace(/className/g, "alert-success");
            $('#btnDelivery').removeAttr("disabled");
        }
        if (type === "VanningOrder") {

            $("#divMessage").html(html);
        } else {
            $("#divMessageCarrierNumber").html(html);
        }
    }

    $("#btnDelivery").bind("click", function () {
        if ($("#btnDelivery").attr("disabled") === "disabled") {
            return;
        }

        $.showIndicator();
        var sysIds = $("#VanningSysId").val();
        if (sysIds.length > 0 && sysIds[0] == ',') {
            sysIds = sysIds.substring(1);
        }
        com.CommonAjax("api/RFOutbound/SaveDeliveryByVanningSysId",
            {
                vanningSysIds: sysIds.split(','),
                currentUserName: GetlocalStorage("DisplayName"),
                currentUserId: GetlocalStorage("UserId"),
                wareHouseSysId: GetlocalStorage("WarehouseSysId")
            },
            function (data) {
                $.hideIndicator();
                if (data != null) {
                    if (data.Message == undefined) {
                        com.SuccessMsg("#msg", "发货完成");
                        $("#divMessageCarrierNumber").html("");
                        $("#divCarrierBox").html("");
                        $("#CarrierNumber").focus();
                        numberList = "";
                    }
                    else {
                        com.FailMsg("#msg", data.Message);
                    }
                }
            });


    });
});

//库存查询
$(document).on("pageInit", "#page-invskuloc-listing", function (e, pageId, $page) {
    $("#skuUPC").focus();
    $("#msg").html("");
    $("#InvSkuLocListing").html("");

    setTimeout(function () {
        $.hideIndicator();
    }, 30000);

    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#InvSkuLocListing").html("");
            var skuUPC = $("#skuUPC").val();
            var loc = $("#loc").val();

            if (!CheckSkuLoc(skuUPC, loc)) {
                return false;
            }

            if (skuUPC != "") {
                $.showIndicator();
                com.CommonAjax("api/RFInventory/GetInvSkuLocList", { SkuUPC: skuUPC, Loc: loc, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
                    if (data != null && data.length > 0) {
                        var jsonArray = new Array();
                        for (var i = 0; i < data.length; i++) {
                            jsonArray.push('<tr><td>' + data[i].SkuName + '</td>');
                            jsonArray.push('<td>' + data[i].Loc + '</td>');
                            jsonArray.push('<td align="right">' + data[i].DisplayQty + '</td>');
                            jsonArray.push('<td align="right">' + data[i].DisplayAvailableQty + '</td></tr> ');
                        }
                        $("#InvSkuLocListing").html(jsonArray.join(''));
                    } else {
                        com.NoData("#InvSkuLocListing");
                    }
                    $("#skuUPC").val("")
                    $.hideIndicator();
                });
            }

        }
    });

    $("#loc").keydown(function () {
        if (event.keyCode == 13) {
            $("#InvSkuLocListing").html("");
            var skuUPC = $("#skuUPC").val();
            var loc = $("#loc").val();

            if (!CheckSkuLoc(skuUPC, loc)) {
                return false;
            }

            if (loc != "") {
                $.showIndicator();
                com.CommonAjax("api/RFInventory/GetInvSkuLocList", { SkuUPC: skuUPC, Loc: loc, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
                    if (data != null && data.length > 0) {
                        var jsonArray = new Array();
                        for (var i = 0; i < data.length; i++) {
                            jsonArray.push('<tr><td>' + data[i].SkuName + '</td>');
                            jsonArray.push('<td>' + data[i].Loc + '</td>');
                            jsonArray.push('<td align="right">' + data[i].DisplayQty + '</td>');
                            jsonArray.push('<td align="right">' + data[i].DisplayAvailableQty + '</td></tr> ');
                        }
                        $("#InvSkuLocListing").html(jsonArray.join(''));
                    } else {
                        com.NoData("#InvSkuLocListing");
                    }
                    $("#loc").val("");
                    $.hideIndicator();
                });
            }

        }
    });

    //验证商品或货位是否存在
    function CheckSkuLoc(skuUPC, loc) {
        var isUPC = false;
        var isLoc = false;

        if (skuUPC != "") {
            com.CommonSyncAjax("api/Base/CheckSkuIsExist",
                {
                    UPC: skuUPC,
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    if (data != null) {
                        if (!data.IsSucess) {
                            com.FailMsg("#msg", data.Message);
                            $("#skuUPC").focus();
                            $("#skuUPC").val("");
                            $("#InvSkuLocListing").html("");
                            isUPC = false;
                        } else {
                            $("#msg").html("");
                            isUPC = true;
                        }
                    }
                });
        } else {
            isUPC = true;
        }

        if (loc != "" && isUPC) {
            com.CommonSyncAjax("api/Base/LocIsExist",
                {
                    LocationSearch: loc,
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    if (data != null) {
                        if (!data.IsSucess) {
                            com.FailMsg("#msg", data.Message);
                            $("#loc").focus();
                            $("#loc").val("");
                            $("#InvSkuLocListing").html("");
                            isLoc = false;
                        } else {
                            $("#msg").html("");
                            isLoc = true;
                        }
                    }
                });
        } else {
            isLoc = true;
        }

        if (isUPC && isLoc) {
            return true;
        } else {
            return false;
        }

    }

});

//待拣货出库单列表
$(document).on("pageInit", "#page-outbound-list", function (e, pageId, $page) {
    $("#outboundOrder").focus();
    $("#msg").html("");
    $("#OutboundPickList").html("");

    //扫描后检查出库单号是否存在
    $("#outboundOrder").keydown(function () {
        if (event.keyCode == 13) {
            var outboundOrder = $("#outboundOrder").val();

            com.CommonAjax("api/PickDetail/CheckOutboundOrder", { OutboundOrder: outboundOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
                if (data.IsSucess) {
                    $("#msg").html("");
                    if (outboundOrder.trim() != "") {
                        $.router.load("PickDetailListing.html?outboundOrder=" + outboundOrder);
                    }
                } else {
                    com.FailMsg("#msg", data.Message);
                    $("#outboundOrder").focus();
                    $("#outboundOrder").val("");
                }
            });
        }
    });

    function WaitingPickOutboundList() {
        //待拣货出库单列表
        loading = true;
        com.CommonLoadDataAjax("api/PickDetail/GetWaitingPickOutboundList",
            {
                WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                iDisplayStart: iDisplayStart,
                iDisplayLength: iDisplayLength
            },
            function (data) {
                if (data.TableResuls.aaData != null && data.TableResuls.aaData.length > 0) {
                    var jsonArray = new Array();
                    for (var i = 0; i < data.TableResuls.aaData.length; i++) {
                        jsonArray.push('<tr onclick="$.router.load(\'PickDetailListing.html?outboundOrder=' + data.TableResuls.aaData[i].OutboundOrder + '\');">');
                        jsonArray.push('<td>' + (iDisplayStart + (i + 1)) + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].OutboundOrder + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].SkuCount + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].SkuQty + '</td></tr> ');
                    }
                    $("#OutboundPickList").append(jsonArray.join(''));
                    iTotalDisplayRecords = data.TableResuls.iTotalDisplayRecords;
                    loading = false;

                    com.NoScroll();
                } else {
                    if (data.TableResuls.iTotalDisplayRecords == 0) {
                        com.NoData("#OutboundPickList");
                    }
                }
            });
    }

    com.ScrollLoadData(WaitingPickOutboundList);
    WaitingPickOutboundList();
});

//拣货清单
$(document).on("pageInit", "#page-pick-listing", function (e, pageId, $page) {
    sku.OnClose();

    $("#PickDetailListing").html("");
    var outboundOrder = com.GetQueryString("outboundOrder");

    com.CommonAjax("api/PickDetail/GetWaitingPickSkuList", { OutboundOrder: outboundOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
        sku.SetSkuDetailList(data);
        if (data != null && data.length > 0) {
            var jsonArray = new Array();
            var totalQty = 0;
            var totalWaitPickQty = 0;
            for (var i = 0; i < data.length; i++) {
                jsonArray.push('<tr>');
                jsonArray.push('<td>' + (i + 1) + '</td>');
                jsonArray.push('<td>' + data[i].UPC + '</td>');
                jsonArray.push('<td>' + data[i].SkuName + '</td>');
                jsonArray.push('<td>' + data[i].WaitPickQty + '</td>');
                totalQty += data[i].Qty;
                totalWaitPickQty += data[i].WaitPickQty;
            }
            var progress = ((totalQty - totalWaitPickQty) / totalQty) * 100;
            $("#progress").html(parseInt(progress))
            $("#PickDetailListing").html(jsonArray.join(''));
        }
    });

    $(".btn").on("click", function () {
        if ($("#progress").html() == "100") {
            alert("拣货已完成");
        } else {
            $.router.load("PickDetail.html?outboundOrder=" + outboundOrder);
        }
    });
});

//扫描拣货
$(document).on("pageInit", "#page-pick-detail", function (e, pageId, $page) {
    var outboundOrder = com.GetQueryString("outboundOrder");

    $("#skuUPC").focus();
    $("#msg").html("");
    $("#skuList").html("");

    //UPC
    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#loc").focus();
        }
    });
    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            if (!sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#loc", null)) {
                com.FailMsg("#msg", "扫描条码在此单据不存在");
                $("#skuUPC").focus();
                $("#skuUPC").val("");
                return false;
            } else {
                $("#msg").html("");
                $("#loc").focus();
            }

            //查询库存
            com.CommonAjax("api/Shelves/GetInventoryList",
                        {
                            SkuSysId: sku.GetSkuSysId(),
                            UPC: $("#skuUPC").val(),
                            WarehouseSysId: GetlocalStorage("WarehouseSysId")
                        },
                        function (data) {
                            if (data != null && data.length > 0) {
                                var jsonArray = new Array();
                                jsonArray.push('提示：当前商品在以下库位中有库存');
                                for (var i = 0; i < data.length; i++) {
                                    jsonArray.push('<div class="row">');
                                    jsonArray.push('<div class="col-xs-5">' + data[i].Loc + '</div>');
                                    jsonArray.push('<div class="col-xs-7">' + data[i].DisplayQty + '</div>');
                                    jsonArray.push('</div>');
                                }
                                $("#skuList").html(jsonArray.join(''));
                            }
                        });

            //com.CommonAjax("api/PickDetail/CheckOutboundDetailSku",
            //    {
            //        OutboundOrder: outboundOrder,
            //        UPC: $("#skuUPC").val(),
            //        WarehouseSysId: GetlocalStorage("WarehouseSysId")
            //    },
            //    function (data) {
            //        if (data != null) {
            //            if (!data.IsSucess) {
            //                com.FailMsg("#msg", data.Message);
            //                $("#skuUPC").focus();
            //                $("#skuUPC").val("");
            //            } else {
            //                $("#msg").html("");
            //                $("#loc").focus();

            //                //查询库存
            //                com.CommonAjax("api/Shelves/GetInventoryList",
            //                            {
            //                                UPC: $("#skuUPC").val(),
            //                                WarehouseSysId: GetlocalStorage("WarehouseSysId")
            //                            },
            //                            function (data) {
            //                                if (data != null && data.length > 0) {
            //                                    var jsonArray = new Array();
            //                                    jsonArray.push('提示：当前商品在以下库位中有库存');
            //                                    for (var i = 0; i < data.length; i++) {
            //                                        jsonArray.push('<div class="row">');
            //                                        jsonArray.push('<div class="col-xs-5">' + data[i].Loc + '</div>');
            //                                        jsonArray.push('<div class="col-xs-7">' + data[i].DisplayQty + '</div>');
            //                                        jsonArray.push('</div>');
            //                                    }
            //                                    $("#skuList").html(jsonArray.join(''));
            //                                }
            //                            });
            //            }
            //        }
            //    });
        }
    });

    //货位
    $("#loc").on("keydown", function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });
    $("#loc").on("blur", function () {
        if ($("#loc").val().trim() != "") {
            com.CommonAjax("api/Base/LocIsExist",
                {
                    LocationSearch: $("#loc").val(),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    if (data != null) {
                        if (!data.IsSucess) {
                            com.FailMsg("#msg", data.Message);
                            $("#loc").focus();
                            $("#loc").val("");
                        } else {
                            $("#msg").html("");
                        }
                    }
                });
        }
    });

    //拣货数量必须为数字
    $("#qty").on("keyup", function () {
        if (!com.CheckPositiveNumber($("#qty").val())) {
            $("#qty").val("");
        }
    });

    $("#qty").on("keydown", function () {
        $("#msg").html("");
        if (event.keyCode == 13) {
            var loc = $("#loc").val();
            var upc = $("#skuUPC").val();
            var qty = $("#qty").val();
            var skuSysId = sku.GetSkuSysId();

            if (upc.trim() == "") {
                com.FailMsg("#msg", "商品条码不能为空");
                $("#skuUPC").focus();
                return false;
            }
            if (loc.trim() == "") {
                com.FailMsg("#msg", "货位不能为空");
                $("#loc").focus();
                return false;
            }
            if (qty.trim() == "") {
                com.FailMsg("#msg", "商品数量不能为空");
                $("#qty").focus();
                return false;
            }
            if (parseInt(qty.trim()) <= 0) {
                com.FailMsg("#msg", "商品数量必须大于0");
                $("#qty").focus();
                return false;
            }
            $.showIndicator();
            com.CommonAjax("api/PickDetail/ScanPickDetail",
                {
                    OutboundOrder: outboundOrder,
                    SkuSysId: skuSysId,
                    UPC: upc,
                    Loc: loc,
                    Qty: qty,
                    CurrentUserId: GetlocalStorage("UserId"),
                    CurrentDisplayName: GetlocalStorage("DisplayName"),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    $.hideIndicator();
                    if (data != null) {
                        if (data.IsSucess) {
                            com.SuccessMsg("#msg", data.Message);
                            $("#skuUPC").val("");
                            $("#loc").val("");
                            $("#qty").val("");
                            $("#skuUPC").focus();
                        } else {
                            com.FailMsg("#msg", data.Message);
                        }
                    }
                });
        }
    });
});

//加工单拣货列表
$(document).on("pageInit", "#page-assembly-pick-list", function (e, pageId, $page) {
    $("#AssemblyOrder").focus();
    $("#msg").html("");
    $("#AssemblyPickList").html("");

    //扫描后检查出库单号是否存在
    $("#AssemblyOrder").keydown(function () {
        if (event.keyCode == 13) {
            var assemblyOrder = $("#AssemblyOrder").val();

            com.CommonAjax("api/PickDetail/CheckAssemblyOrder", { AssemblyOrderSearch: assemblyOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
                if (data.IsSucess) {
                    $("#msg").html("");
                    if (assemblyOrder.trim() != "") {
                        $.router.load("AssemblyPickListing.html?assemblyOrder=" + assemblyOrder);
                    }
                } else {
                    com.FailMsg("#msg", data.Message);
                    $("#assemblyOrder").focus();
                    $("#assemblyOrder").val("");
                }
            });
        }
    });

    function WaitingAssemblyPickList() {
        //待拣货出库单列表
        loading = true;
        com.CommonLoadDataAjax("api/PickDetail/GetWaitingAssemblyList",
            {
                WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                iDisplayStart: iDisplayStart,
                iDisplayLength: iDisplayLength
            },
            function (data) {
                if (data.TableResuls.aaData != null && data.TableResuls.aaData.length > 0) {
                    var jsonArray = new Array();
                    for (var i = 0; i < data.TableResuls.aaData.length; i++) {
                        jsonArray.push('<tr onclick="$.router.load(\'AssemblyPickListing.html?assemblyOrder=' + data.TableResuls.aaData[i].AssemblyOrder + '\');">');
                        jsonArray.push('<td>' + (iDisplayStart + (i + 1)) + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].AssemblyOrder + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].SkuCount + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].DisplaySkuQty + '</td></tr> ');
                    }
                    $("#AssemblyPickList").append(jsonArray.join(''));
                    iTotalDisplayRecords = data.TableResuls.iTotalDisplayRecords;
                    loading = false;

                    com.NoScroll();
                } else {
                    if (data.TableResuls.iTotalDisplayRecords == 0) {
                        com.NoData("#AssemblyPickList");
                    }
                }
            });
    }

    com.ScrollLoadData(WaitingAssemblyPickList);
    WaitingAssemblyPickList();
});

//加工单拣货清单
$(document).on("pageInit", "#page-assembly-pick-listing", function (e, pageId, $page) {
    sku.OnClose();

    $("#AssemblyPickDetailListing").html("");
    var assemblyOrder = com.GetQueryString("assemblyOrder");

    com.CommonAjax("api/PickDetail/GetWaitingAssemblyPickSkuList", { AssemblyOrder: assemblyOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
        sku.SetSkuDetailList(data);
        if (data != null && data.length > 0) {
            var jsonArray = new Array();
            var totalQty = 0;
            var totalWaitPickQty = 0;
            for (var i = 0; i < data.length; i++) {
                jsonArray.push('<tr>');
                jsonArray.push('<td>' + (i + 1) + '</td>');
                jsonArray.push('<td>' + data[i].UPC + '</td>');
                jsonArray.push('<td>' + data[i].SkuName + '</td>');
                jsonArray.push('<td>' + data[i].DisplayWaitPickQty + '</td>');
                totalQty += data[i].DisplayQty;
                totalWaitPickQty += data[i].DisplayWaitPickQty;
            }
            var progress = ((totalQty - totalWaitPickQty) / totalQty) * 100;
            $("#progress").html(parseInt(progress))
            $("#AssemblyPickDetailListing").html(jsonArray.join(''));
        }
    });

    $(".btn").on("click", function () {
        if ($("#progress").html() == "100") {
            alert("拣货已完成");
        } else {
            $.router.load("AssemblyPickDetail.html?assemblyOrder=" + assemblyOrder);
        }
    });
});

//加工单扫描拣货
$(document).on("pageInit", "#page-assembly-pick-detail", function (e, pageId, $page) {
    var assemblyOrder = com.GetQueryString("assemblyOrder");

    $("#skuUPC").focus();
    $("#msg").html("");
    $("#skuList").html("");

    //UPC
    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#loc").focus();
        }
    });
    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            if (!sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#loc", null, function () {
                //查询库存
                com.CommonAjax("api/Shelves/GetInventoryList", {
                SkuSysId: sku.GetSkuSysId(),
                UPC: $("#skuUPC").val(),
                WarehouseSysId: GetlocalStorage("WarehouseSysId")
            }, function (data) {
                    if (data != null && data.length > 0) {
                        var jsonArray = new Array();
                        jsonArray.push('提示：当前商品在以下库位中有库存');
                        for (var i = 0; i < data.length; i++) {
                            jsonArray.push('<div class="row">');
                            jsonArray.push('<div class="col-xs-5">' + data[i].Loc + '</div>');
                            jsonArray.push('<div class="col-xs-7">' + data[i].DisplayQty + '</div>');
                            jsonArray.push('</div>');
            }
                        $("#skuList").html(jsonArray.join(''));
            }
            });
            })) {
                com.FailMsg("#msg", "扫描条码在此单据不存在");
                $("#skuUPC").focus();
                $("#skuUPC").val("");
                return false;
            } else {
                $("#msg").html("");
                $("#loc").focus();
            }

            //com.CommonAjax("api/PickDetail/CheckAssemblyDetailSku",
            //    {
            //        AssemblyOrder: assemblyOrder,
            //        UPC: $("#skuUPC").val(),
            //        WarehouseSysId: GetlocalStorage("WarehouseSysId")
            //    },
            //    function (data) {
            //        if (data != null) {
            //            if (!data.IsSucess) {
            //                com.FailMsg("#msg", data.Message);
            //                $("#skuUPC").focus();
            //                $("#skuUPC").val("");
            //            } else {
            //                $("#msg").html("");
            //                $("#loc").focus();

            //                //查询库存
            //                com.CommonAjax("api/Shelves/GetInventoryList",
            //                            {
            //                                UPC: $("#skuUPC").val(),
            //                                WarehouseSysId: GetlocalStorage("WarehouseSysId")
            //                            },
            //                            function (data) {
            //                                if (data != null && data.length > 0) {
            //                                    var jsonArray = new Array();
            //                                    jsonArray.push('提示：当前商品在以下库位中有库存');
            //                                    for (var i = 0; i < data.length; i++) {
            //                                        jsonArray.push('<div class="row">');
            //                                        jsonArray.push('<div class="col-xs-5">' + data[i].Loc + '</div>');
            //                                        jsonArray.push('<div class="col-xs-7">' + data[i].DisplayQty + '</div>');
            //                                        jsonArray.push('</div>');
            //                                    }
            //                                    $("#skuList").html(jsonArray.join(''));
            //                                }
            //                            });
            //            }
            //        }
            //    });
        }
    });

    //货位
    $("#loc").on("keydown", function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });
    $("#loc").on("blur", function () {
        if ($("#loc").val().trim() != "") {
            com.CommonAjax("api/Base/LocIsExist", {
                LocationSearch: $("#loc").val(),
                WarehouseSysId: GetlocalStorage("WarehouseSysId")
            }, function (data) {
                if (data != null) {
                    if (!data.IsSucess) {
                        com.FailMsg("#msg", data.Message);
                        $("#loc").focus();
                        $("#loc").val("");
                    } else {
                        $("#msg").html("");
                    }
                }
            });
        }
    });

    //拣货数量必须为数字
    $("#qty").on("keyup", function () {
        if (!com.CheckNumber($("#qty").val())) {
            $("#qty").val("");
        }
    });

    $("#qty").on("keydown", function () {
        $("#msg").html("");
        if (event.keyCode == 13) {
            var loc = $("#loc").val();
            var upc = $("#skuUPC").val();
            var qty = $("#qty").val();
            var skuSysId = sku.GetSkuSysId();

            if (upc.trim() == "") {
                com.FailMsg("#msg", "商品条码不能为空");
                $("#skuUPC").focus();
                return false;
            }
            if (loc.trim() == "") {
                com.FailMsg("#msg", "货位不能为空");
                $("#loc").focus();
                return false;
            }
            if (qty.trim() == "") {
                com.FailMsg("#msg", "商品数量不能为空");
                $("#qty").focus();
                return false;
            }
            if (parseFloat(qty.trim()) <= 0) {
                com.FailMsg("#msg", "商品数量必须大于0");
                $("#qty").focus();
                return false;
            }
            $.showIndicator();
            com.CommonAjax("api/PickDetail/AssemblyScanPickDetail",
                {
                    AssemblyOrder: assemblyOrder,
                    SkuSysId: skuSysId,
                    UPC: upc,
                    Loc: loc,
                    DisplayQty: qty,
                    CurrentUserId: GetlocalStorage("UserId"),
                    CurrentDisplayName: GetlocalStorage("DisplayName"),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    $.hideIndicator();
                    $("#skuList").html("");
                    if (data != null) {
                        if (data.IsSucess) {
                            com.SuccessMsg("#msg", data.Message);
                            $("#skuUPC").val("");
                            $("#loc").val("");
                            $("#qty").val("");
                            $("#skuUPC").focus();
                        } else {
                            com.FailMsg("#msg", data.Message);
                        }
                    }
                });
        }
    });
});


//待上架加工单列表
$(document).on("pageInit", "#page-assemblyshelves-index", function (e, pageId, $page) {
    $("#order").focus();
    $("#msg").html("");
    $("#AssemblyShelvesIndexList").html("");

    $("#order").keydown(function () {
        if (event.keyCode == 13) {
            var assemblyOrder = $("#order").val();

            com.CommonGetAjax("api/Shelves/CheckAssemblyOrderNotOnShelves" + "?assemblyOrder=" + assemblyOrder + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
                if (data.IsSucess) {
                    $("#msg").html("");
                    if (assemblyOrder.trim() != "") {
                        $.router.load("AssemblyShelvesListing.html?assemblyOrder=" + assemblyOrder);
                    }
                } else {
                    com.FailMsg("#msg", data.Message);
                    $("#order").focus();
                    $("#order").val("");
                }
            });
        }
    });

    function AssemblyWaitingShelvesList() {
        loading = true;
        com.CommonLoadDataAjax("api/Shelves/GetAssemblyWaitingShelvesList",
            {
                WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                iDisplayStart: iDisplayStart,
                iDisplayLength: iDisplayLength
            },
            function (data) {
                if (data.TableResuls.aaData != null && data.TableResuls.aaData.length > 0) {
                    var jsonArray = new Array();
                    for (var i = 0; i < data.TableResuls.aaData.length; i++) {
                        jsonArray.push('<tr onclick="$.router.load(\'AssemblyShelvesListing.html?assemblyOrder=' + data.TableResuls.aaData[i].AssemblyOrder + '\');">');
                        jsonArray.push('<td>' + (iDisplayStart + (i + 1)) + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].AssemblyOrder + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].SkuNumber + '</td>');
                        jsonArray.push('<td>' + data.TableResuls.aaData[i].SkuQty + '</td></tr> ');
                    }
                    $("#AssemblyShelvesIndexList").append(jsonArray.join(''));
                    iTotalDisplayRecords = data.TableResuls.iTotalDisplayRecords;
                    loading = false;

                    com.NoScroll();
                } else {
                    if (data.TableResuls.iTotalDisplayRecords == 0) {
                        com.NoData("#AssemblyShelvesIndexList");
                    }
                }
            });
    }

    com.ScrollLoadData(AssemblyWaitingShelvesList);
    AssemblyWaitingShelvesList();
});

//加工单上架清单
$(document).on("pageInit", "#page-assemblyshelves-listing", function (e, pageId, $page) {
    sku.OnClose();

    $("#AssemblyShelvesListing").html("");
    var assemblyOrder = com.GetQueryString("assemblyOrder");

    com.CommonAjax("api/Shelves/GetAssemblyWaitingShelvesSkuList", { AssemblyOrder: assemblyOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
        sku.SetSkuDetailList(data);
        if (data != null && data.length > 0) {
            var jsonArray = new Array();
            var totalQty = 0;
            var totalSkuQty = 0;
            for (var i = 0; i < data.length; i++) {
                jsonArray.push('<tr>');
                jsonArray.push('<td>' + (i + 1) + '</td>');
                jsonArray.push('<td>' + data[i].UPC + '</td>');
                jsonArray.push('<td>' + data[i].SkuName + '</td>');
                jsonArray.push('<td>' + data[i].SkuQty + '</td>');
                totalQty += data[i].ActualQty;
                totalSkuQty += data[i].SkuQty;
            }
            var progress = ((totalQty - totalSkuQty) / totalQty) * 100;
            $("#progress").html(parseInt(progress))
            $("#AssemblyShelvesListing").html(jsonArray.join(''));
        }
    });

    $(".btn").on("click", function () {
        if ($("#progress").html() == "100") {
            alert("上架已完成");
        } else {
            $.router.load("AssemblyShelvesDetail.html?assemblyOrder=" + assemblyOrder);
        }
    });
});

//加工单扫描上架
$(document).on("pageInit", "#page-assemblyshelves-detail", function (e, pageId, $page) {
    var assemblyOrder = com.GetQueryString("assemblyOrder");

    $("#skuUPC").focus();
    $("#msg").html("");
    $("#skuList").html("");

    //UPC
    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#loc").focus();
        }
    });
    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            if (!sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#loc", null)) {
                com.FailMsg("#msg", "扫描条码在此单据不存在");
                $("#skuUPC").focus();
                $("#skuUPC").val("");
                return false;
            } else {
                $("#msg").html("");
                $("#loc").focus();
            }

            //查询库存
            com.CommonAjax("api/Shelves/GetInventoryList", {
                UPC: $("#skuUPC").val(),
                SkuSysId: sku.GetSkuSysId(),
                WarehouseSysId: GetlocalStorage("WarehouseSysId")
            }, function (data) {
                if (data != null && data.length > 0) {
                    var jsonArray = new Array();
                    jsonArray.push('提示：当前商品在以下库位中有库存');
                    for (var i = 0; i < data.length; i++) {
                        jsonArray.push('<div class="row">');
                        jsonArray.push('<div class="col-xs-5">' + data[i].Loc + '</div>');
                        jsonArray.push('<div class="col-xs-7">' + data[i].Qty + '</div>');
                        jsonArray.push('</div>');
                    }
                    $("#skuList").html(jsonArray.join(''));
                }
            });


            //com.CommonAjax("api/Shelves/CheckAssemblyWaitShelvesSku",
            //    {
            //        AssemblyOrder: assemblyOrder,
            //        UPC: $("#skuUPC").val(),
            //        WarehouseSysId: GetlocalStorage("WarehouseSysId")
            //    },
            //    function (data) {
            //        if (data != null) {
            //            if (!data.IsSucess) {
            //                com.FailMsg("#msg", data.Message);
            //                $("#skuUPC").focus();
            //                $("#skuUPC").val("");
            //            } else {
            //                $("#msg").html("");
            //                $("#loc").focus();

            //                //查询库存
            //                com.CommonAjax("api/Shelves/GetInventoryList",
            //                            {
            //                                UPC: $("#skuUPC").val(),
            //                                WarehouseSysId: GetlocalStorage("WarehouseSysId")
            //                            },
            //                            function (data) {
            //                                if (data != null && data.length > 0) {
            //                                    var jsonArray = new Array();
            //                                    jsonArray.push('提示：当前商品在以下库位中有库存');
            //                                    for (var i = 0; i < data.length; i++) {
            //                                        jsonArray.push('<div class="row">');
            //                                        jsonArray.push('<div class="col-xs-5">' + data[i].Loc + '</div>');
            //                                        jsonArray.push('<div class="col-xs-7">' + data[i].Qty + '</div>');
            //                                        jsonArray.push('</div>');
            //                                    }
            //                                    $("#skuList").html(jsonArray.join(''));
            //                                }
            //                            });
            //            }
            //        }
            //    });
        }
    });

    //货位
    $("#loc").on("keydown", function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });

    $("#loc").on("blur", function () {
        if ($("#loc").val().trim() != "") {
            com.CommonAjax("api/Base/LocIsExist", {
                LocationSearch: $("#loc").val(),
                WarehouseSysId: GetlocalStorage("WarehouseSysId")
            }, function (data) {
                if (data != null) {
                    if (!data.IsSucess) {
                        com.FailMsg("#msg", data.Message);
                        $("#loc").focus();
                        $("#loc").val("");
                    } else {
                        $("#msg").html("");
                    }
                }
            });
        } else {
            $("#loc").focus();
            $("#loc").val("");
        }
    });


    //上架数量必须为数字
    $("#qty").on("keyup", function () {
        if (!com.CheckPositiveNumber($("#qty").val())) {
            $("#qty").val("");
        }
    });

    $("#qty").on("keydown", function () {
        $("#msg").html("");
        if (event.keyCode == 13) {
            var loc = $("#loc").val();
            var upc = $("#skuUPC").val();
            var qty = $("#qty").val();
            var skuSysId = sku.GetSkuSysId();

            if (upc.trim() == "") {
                com.FailMsg("#msg", "商品条码不能为空");
                $("#skuUPC").focus();
                return false;
            }
            if (loc.trim() == "") {
                com.FailMsg("#msg", "货位不能为空");
                $("#loc").focus();
                return false;
            }
            if (qty.trim() == "") {
                com.FailMsg("#msg", "商品数量不能为空");
                $("#qty").focus();
                return false;
            }
            if (parseInt(qty.trim()) <= 0) {
                com.FailMsg("#msg", "商品数量必须大于0");
                $("#qty").focus();
                return false;
            }

            $.showIndicator();
            com.CommonAjax("api/Shelves/AssemblyScanShelves",
                {
                    AssemblyOrder: assemblyOrder,
                    SkuSysId: skuSysId,
                    UPC: upc,
                    Loc: loc,
                    Qty: qty,
                    UserId: GetlocalStorage("UserId"),
                    CurrentDisplayName: GetlocalStorage("DisplayName"),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    $.hideIndicator();
                    if (data != null) {
                        if (data.IsSucess) {
                            com.SuccessMsg("#msg", data.Message);
                            $("#skuUPC").val("");
                            $("#loc").val("");
                            $("#qty").val("");
                            $("#skuUPC").focus();
                        } else {
                            com.FailMsg("#msg", data.Message);
                        }
                    }
                });
        }
    });
});

//出库复核
$(document).on("pageInit", "#page-outboundreview-list", function (e, pageId, $page) {
    sku.OnClose();

    $("#outboundOrder").focus();
    $("#msg").html("");
    $("#OutboundReviewList").html("");
    var isCached;

    //扫描后检查出库单号是否存在
    $("#outboundOrder").keydown(function () {
        if (event.keyCode == 13) {
            $("#OutboundReviewList").html("");
            var outboundOrder = $("#outboundOrder").val();
            com.CommonAjax("api/RFOutbound/CheckOutboundExists", { OutboundOrder: outboundOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
                if (data.IsSucess) {
                    $("#msg").html("");
                    if (outboundOrder.trim() != "") {
                        com.CommonAjax("api/RFOutbound/GetWaitingReviewList", {
                            OutboundOrder: outboundOrder,
                            WarehouseSysId: GetlocalStorage("WarehouseSysId")
                        }, function (data) {
                            if (data != null) {
                                sku.SetSkuDetailList(data.WaitingReviewList);
                                isCached = data.IsCached;
                                if (data.WaitingReviewList != null && data.WaitingReviewList.length > 0) {
                                    var jsonArray = new Array();
                                    for (var i = 0; i < data.WaitingReviewList.length; i++) {
                                        jsonArray.push('<tr>');
                                        jsonArray.push('<td>' + (i + 1) + '</td>');
                                        jsonArray.push('<td>' + data.WaitingReviewList[i].UPC + '</td>');
                                        jsonArray.push('<td>' + data.WaitingReviewList[i].SkuName + '</td>');
                                        jsonArray.push('<td>' + data.WaitingReviewList[i].SkuQty + '</td></tr> ');
                                    }
                                    $("#OutboundReviewList").append(jsonArray.join(''));
                                } else {
                                    if (data.WaitingReviewList.length == 0) {
                                        com.NoData("#OutboundReviewList");
                                    }
                                }
                            }
                        });
                    }
                } else {
                    com.FailMsg("#msg", data.Message);
                    $("#outboundOrder").focus();
                    $("#outboundOrder").val("");
                }
            });
        }
    });

    $("#btnStartReview").on("click", function () {
        if ($("#outboundOrder").val().trim() != "" && $("#OutboundReviewList").children("tr").length > 0) {
            $.router.load("ReviewScanSku.html?outboundOrder=" + $("#outboundOrder").val());
        } else {
            com.FailMsg("#msg", "请扫描单据条码");
            $("#outboundOrder").focus();
        }
    });

    $("#btnReset").on("click", function () {
        $("#outboundOrder").focus();
        $("#outboundOrder").val("");
        $("#OutboundReviewList").html("");
        $("#msg").html("");
    });
});

//复核扫描
$(document).on("pageInit", "#page-outboundreview-scansku", function (e, pageId, $page) {
    com.hideLoading();
    $("#skuUPC").focus();
    $("#msg").html("");
    $("#OutboundScanningList").html("");
    $("#txt_SkuCount").text("0");
    $("#txt_PackCount").text("0");
    var outboundOrder = com.GetQueryString("outboundOrder");
    var scanSkus = new Array();
    var scanPacks = new Array();

    $(function () {
        var waitingScanSkuList = sku.SkuDetailList;
        for (var i = 0; i < waitingScanSkuList.length; i++) {
            var item = '<tr>';
            item += '<td>' + ($("#OutboundScanningList").children("tr").length + 1) + '</td>';
            item += '<td style="display:none">' + waitingScanSkuList[i].SkuSysId + '</td>';
            item += '<td>' + waitingScanSkuList[i].UPC + '</td>';
            item += '<td>' + waitingScanSkuList[i].SkuName + '</td>';
            item += '<td></td>';
            item += '</tr>';
            $("#OutboundScanningList").append(item);
        }
    });

    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });

    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            //if (!sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#qty", null)) {
            //    com.FailMsg("#msg", "条码错误，出库单不包含该商品，请重新复核！");
            //    $("#skuUPC").focus();
            //    $("#skuUPC").val("");
            //    return false;
            //} else {
            //    $("#msg").html("");
            //    $("#qty").focus();
            //}
            sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#qty", null, null)
            $("#msg").html("");
            $("#qty").focus();
        }
    });

    $("#qty").on("keyup", function () {
        if (!com.CheckNumber($("#qty").val())) {
            $("#qty").val("");
        }
    });

    $("#qty").keydown(function () {
        if (event.keyCode == 13) {
            if ($("#skuUPC").val().trim() != "" && $("#qty").val().trim() != "") {
                $("#msg").html("");
                com.CommonAjax("api/RFOutbound/CheckReviewDetailSku", {
                    OutboundOrder: outboundOrder,
                    WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                    SkuSysId: sku.GetSkuSysId(),
                    Qty: $("#qty").val(),
                    SkuUPC: $("#skuUPC").val().trim()
                }, function (data) {
                    if (data != null) {
                        if (data.RFCommResult.IsSucess) {
                            for (var i = 0; i < data.Skus.length; i++) {
                                var checkedSku = data.Skus[i];
                                $("#OutboundScanningList").find("tr").each(function () {
                                    var tds = $(this).children();
                                    if (tds.eq(1).text() == checkedSku.SkuSysId) {
                                        tds.eq(4).text(checkedSku.DisplaySkuQty);
                                        var upcFlag = $("#selectkey").val().split('|')[0];
                                        if (upcFlag == "P") {
                                            scanPacks.push($("#selectkey").val());
                                        } else {
                                            scanSkus.push(checkedSku.SkuSysId);
                                        }
                                        return false;
                                    }
                                });
                            }
                            $("#skuUPC").val("");
                            $("#skuUPC").focus();
                            $("#qty").val("");
                            $("#txt_SkuCount").text(getCount(scanSkus));
                            $("#txt_PackCount").text(getCount(scanPacks));
                        } else {
                            com.FailMsg("#msg", data.RFCommResult.Message);
                            $("#skuUPC").focus();
                            $("#skuUPC").val("");
                            $("#qty").val("");
                        }
                    }
                });
            }
        }
    });

    $("#txt_DetailList").on("click", function () {
        if ($("#icon_DetailList").hasClass("fa fa-chevron-down")) {
            $("#icon_DetailList").attr("class", "fa fa-chevron-up");
            $("#div_DetailList").show("normal");
        } else {
            $("#icon_DetailList").attr("class", "fa fa-chevron-down");
            $("#div_DetailList").hide("normal");
        }
    });

    $("#btnFinishReview").on("click", function () {
        scanSkus = new Array();
        scanPacks = new Array();
        //$.router.load("ReviewFinish.html?outboundOrder=" + outboundOrder + "&skuCount=" + $("#txt_SkuCount").text() + "&packCount=" + $("#txt_PackCount").text());
        $.router.load("TransferOrderReviewDiff.html?outboundOrder=" + outboundOrder);
    });

    function getCount(arr) {
        var n = [];
        for (var i = 0; i < arr.length; i++) {
            if (n.indexOf(arr[i]) == -1) n.push(arr[i]);
        }
        return n.length;
    }
});

//复核完成
$(document).on("pageInit", "#page-outboundreview-finish", function (e, pageId, $page) {
    $("#OutboundFinishList1").html("");
    $("#OutboundFinishList2").html("");
    var outboundOrder = com.GetQueryString("outboundOrder");
    var warehouseSysId = GetlocalStorage("WarehouseSysId");
    var skuCount = com.GetQueryString("skuCount");
    var packCount = com.GetQueryString("packCount");

    $(function () {
        $("#txt_SkuCount1").text(skuCount);
        $("#txt_PackCount1").text(packCount);
        $("#txt_SkuCount2").text(skuCount);
        $("#txt_PackCount2").text(packCount);
    });

    function getReviewFinishResult() {
        com.CommonAjax("api/RFOutbound/GetReviewFinishResult",
            {
                OutboundOrder: outboundOrder,
                WarehouseSysId: warehouseSysId
            }, function (data) {
                if (data != null) {
                    setTabData("#OutboundFinishList1", data.NoScanSkus);
                    setTabData("#OutboundFinishList2", data.QtyReducedSkus);
                }
            });
    }

    function setTabData(id, data) {
        if (data.length > 0) {
            var jsonArray = new Array();
            for (var i = 0; i < data.length; i++) {
                jsonArray.push('<tr>');
                jsonArray.push('<td>' + (i + 1) + '</td>');
                jsonArray.push('<td>' + data[i].SkuName + '</td>');
                jsonArray.push('<td>' + data[i].SkuUPC + '</td>');
                jsonArray.push('<td>' + data[i].OutboundQty + '</td>');
                jsonArray.push('<td>' + data[i].DisplayQty + '</td></tr> ');
            }
            $(id).append(jsonArray.join(''));
        }
    }

    getReviewFinishResult();
    $("#loading").remove();

    $("#btnBackMainMenu").on("click", function () {
        $.router.load("../../Home.html");
    });
});

//预包装主页
$(document).on("pageInit", "#page-prepack-index", function (e, pageId, $page) {
    sku.OnClose();
    $("#inputQty").val("");
});

//整件预包装
$(document).on("pageInit", "#page-delivery-prepack", function (e, pageId, $page) {
    $("#PrePackDetailList").html("");
    $("#detailList").hide();
    $("#icon_DetailList").removeClass("fa-chevron-up").addClass("fa-chevron-down");
    $("#skuCount").html(0);
    ClearInput();
    $("#storageLoc").focus();
    $("#msg").html("");
    var scanSkus = new Array();

    $("input[name='mode']").on("click", function () {
        $("#storageLoc").focus();
    });

    //库位
    $("#storageLoc").on("keydown", function () {
        if (event.keyCode == 13) {
            $("#skuUPC").focus();
            $("#skuCount").html("0");
            scanSkus = new Array();
        }
    });
    $("#storageLoc").on("blur", function () {
        if ($("#storageLoc").val().trim() != "") {
            $("#PrePackDetailList").html("");
            $("#msg").html("");
            com.CommonAjax("api/RFOutbound/GetPrePackByQuery",
                {
                    StorageLoc: $("#storageLoc").val(),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    if (data != null) {
                        $("#skuUPC").focus();
                        PrePackDetailList();
                    } else {
                        $("#storageLoc").val("");
                        $("#storageLoc").focus();
                        com.FailMsg("#msg", "库位不存在");
                    }
                });
        }
    });

    //预包装明细
    function PrePackDetailList() {
        com.CommonAjax("api/RFOutbound/GetPrePackDetailList",
            {
                StorageLoc: $("#storageLoc").val(),
                WarehouseSysId: GetlocalStorage("WarehouseSysId")
            },
            function (data) {
                sku.SetSkuDetailList(data);
                if (data != null && data.length > 0) {
                    var jsonArray = new Array();
                    for (var i = 0; i < data.length; i++) {
                        jsonArray.push('<tr>');
                        jsonArray.push('<td>' + (i + 1) + '</td>');
                        jsonArray.push('<td>' + data[i].UPC + '</td>');
                        jsonArray.push('<td>' + data[i].SkuName + '</td>');
                        jsonArray.push('<td>' + data[i].PreQty + '</td>');
                        jsonArray.push('<td>' + data[i].Qty + '</td>');
                        jsonArray.push('<td>' + data[i].UOMCode + '</td></tr> ');
                    }
                    $("#PrePackDetailList").append(jsonArray.join(''));
                    //$("#skuCount").html(data.length);
                }
            });
    }

    //UPC
    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });
    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#qty", function () {
                if ($("#qty").val() == "") {
                    $("#qty").val(1)
                }
            });
            $("#msg").html("");

            com.CommonNoLoadingAjax("api/RFOutbound/CheckPrePackDetailSku",
                {
                    SkuSysId: sku.GetSkuSysId(),
                    UPC: $("#skuUPC").val(),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    if (data != null) {
                        if (!data.IsSucess) {
                            com.FailMsg("#msg", data.Message);
                            $("#skuUPC").focus();
                            $("#skuUPC").val("");
                        } else {
                            $("#msg").html("");
                            $("#qty").focus();
                        }
                    }
                });
        }
    });

    //商品数量必须为数字
    $("#qty").on("keyup", function () {
        if (!com.CheckNumber($("#qty").val())) {
            $("#qty").val("");
        }
    });
    $("#qty").on("keydown", function () {
        if (event.keyCode == 13) {
            var storageLoc = $("#storageLoc").val();
            var upc = $("#skuUPC").val();
            var qty = $("#qty").val();

            if (storageLoc.trim() == "") {
                com.FailMsg("#msg", "库位不能为空");
                $("#storageLoc").focus();
                return false;
            }
            if (upc.trim() == "") {
                com.FailMsg("#msg", "商品/箱号不能为空");
                $("#skuUPC").focus();
                return false;
            }
            if (qty.trim() == "") {
                com.FailMsg("#msg", "商品数量不能为空");
                $("#qty").focus();
                return false;
            }
            if (parseInt(qty.trim()) <= 0) {
                com.FailMsg("#msg", "商品数量必须大于0");
                $("#qty").focus();
                return false;
            }
            $.showIndicator();
            com.CommonNoLoadingAjax("api/RFOutbound/ScanPrePack",
                {
                    StorageLoc: storageLoc,
                    SkuSysId: sku.GetSkuSysId(),
                    UPC: upc,
                    Qty: qty,
                    CurrentUserId: GetlocalStorage("UserId"),
                    CurrentDisplayName: GetlocalStorage("DisplayName"),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    $.hideIndicator();
                    if (data != null) {
                        if (data.IsSucess) {
                            $("#PrePackDetailList").html("");
                            PrePackDetailList();

                            if ($j('input:radio[name="mode"]:checked').val() == "2") {
                                $("#skuUPC").val("");
                                $("#qty").val("1");
                                $("#skuUPC").focus();
                            } else {
                                ClearInput();
                                $("#storageLoc").focus();
                            }
                            var upcFlag = $("#selectkey").val().split('|')[0];
                            if (upcFlag == "S") {
                                scanSkus.push($("#selectkey").val());
                                $("#skuCount").text(getCount(scanSkus));
                            }
                            com.SuccessMsg("#msg", "UPC/箱号：" + upc + "，数量：" + qty + "，库位：" + storageLoc + "，预包装成功！");
                        } else {
                            com.FailMsg("#msg", data.Message);
                        }
                    }
                });
        }
    });

    function ClearInput() {
        $("#storageLoc").val("");
        $("#skuUPC").val("");
        $("#qty").val("1");
    }

    $("#txt_DetailList").on("click", function () {
        if ($j("#detailList").is(":hidden")) {
            $("#detailList").show("normal");
            $("#icon_DetailList").removeClass("fa-chevron-down").addClass("fa-chevron-up");
        } else {
            $("#detailList").hide("normal");
            $("#icon_DetailList").removeClass("fa-chevron-up").addClass("fa-chevron-down");
        }
    });

    function getCount(arr) {
        var n = [];
        for (var i = 0; i < arr.length; i++) {
            if (n.indexOf(arr[i]) == -1) n.push(arr[i]);
        }
        return n.length;
    }
});

//散货预包装
$(document).on("pageInit", "#page-prebulkpack-index", function (e, pageId, $page) {
    $("#storageCase").focus();
    $("#msg").html("");
    $("#PreBulkPackNoScanList").html("");
    $("#PreBulkPackDetailList").html("");
    $("#txt_SkuCount").html("");
    $("#qty").val("1");
    $("#inputQty").val("");

    $("#storageCase").keydown(function () {
        if (event.keyCode == 13) {
            $("#msg").html("");
            if ($("#storageCase").val().trim() != "") {
                getPreBulkPackDetails(false);
                $("#skuUPC").focus();
            } else {
                com.FailMsg("#msg", "请扫描箱号");
                //$("#msg").html("请扫描箱号");
            }
        }
    });

    $("#qty").on("keyup", function () {
        if (!com.CheckNumber($("#qty").val())) {
            $("#qty").val("1");
            $("#qty").focus();
        }
        $("#inputQty").val($("#qty").val());
    });

    $("#qty").keydown(function () {
        if (event.keyCode == 13) {
            $("#msg").html("");
            $("#skuUPC").focus();
        }
        $("#inputQty").val($("#qty").val());
    });
    var submitFlag = true;
    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13 && submitFlag == true) {
            submitFlag = false;
            if ($("#skuUPC").val().trim() != "" && $("#qty").val().trim() != "") {
                com.CommonNoLoadingGetAjax("api/RFOutbound/CheckPreBulkPackDetailSku?upc=" + $("#skuUPC").val().trim() + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
                    if (data != null) {
                        if (data.RFCommResult.IsSucess) {
                            sku.SetSkuDetailList(data.Skus);
                            var inputQty = 0;
                            if ($("#qty").val() != null && $("#qty").val().trim != "") {
                                inputQty = $("#qty").val();
                            }
                            if (sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#skuUPC", function () {
                                if ($("#qty").val() == 1) {
                                    $("#qty").val(inputQty);
                            }
                                if ($("#qty").val() == "") {
                                    $("#qty").val(1);
                            }
                            }, function () {
                                $("#msg").html("");
                                var isQtyExceeded = false;
                                var preQty = 0;
                                $("#PreBulkPackDetailList").find("tr").each(function () {
                                    var tds = $(this).children();
                                    var rowSkuSysId = tds.eq(6).text();
                                    var rowPreQty = parseInt(tds.eq(3).text());
                                    var rowQty = parseInt(tds.eq(4).text());
                                    if (sku.GetSkuSysId() == rowSkuSysId) {
                                        if (!(isNaN(rowPreQty)) && inputQty > rowPreQty - rowQty) {
                                            isQtyExceeded = true;
                            }
                                        return false;
                            }
                            });

                                if (isQtyExceeded == false) {
                                    com.CommonAjax("api/RFOutbound/GeneratePreBulkPackDetail", {
                                StorageCase: $("#storageCase").val().trim(),
                                SkuSysId: sku.GetSkuSysId(),
                                UPC: $("#skuUPC").val().trim(),
                                Qty: $("#qty").val(),
                                WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                                CurrentUserId: GetlocalStorage("UserId"),
                                CurrentDisplayName: GetlocalStorage("DisplayName")
                            }, function (data) {
                                                            if (data.IsSucess) {
                                                                submitFlag = true;
                                                                $("#inputQty").val("");
                                                                com.SuccessMsg("#msg", data.Message);
                                                                getPreBulkPackDetails(true);
                            } else {
                                                                submitFlag = true;
                                                                com.FailMsg("#msg", data.Message);
                                                                $("#loading").remove();
                            }
                                                            var mode = $("input[type='radio']:checked").val();
                                                            if (mode == 1) { //交替模式
                                                                $("#storageCase").focus();
                                                                $("#storageCase").val("");
                                                                $("#skuUPC").val("");
                                                                $("#qty").val("1");
                            } else if (mode == 2) { //连续模式
                                                                $("#skuUPC").focus();
                                                                $("#skuUPC").val("");
                                                                $("#qty").val("1");
                            }
                            });
                            } else {
                                        submitFlag = true;
                                        com.FailMsg("#msg", "扫描数量不能大于剩余数量");
                                        $("#qty").focus();
                            }
                            })) {
                                var aaa = "ad";
                            } else {
                                submitFlag = true;
                                com.FailMsg("#msg", "扫描条码在此单据不存在");
                                $("#skuUPC").focus();
                                $("#skuUPC").val("");
                                return false;
                            }
                        } else {
                            submitFlag = true;
                            com.FailMsg("#msg", data.RFCommResult.Message);
                            $("#skuUPC").focus();
                            $("#skuUPC").val(""); $("#loading").remove();
                        }
                    }
                });
            }
        }
    });

    function pickerCloseCallBack() {

    }

    $("input[type='radio']").on("click", function () {
        $("#storageCase").focus();
    });

    $("#btnReset").on('click', function () {
        $("#storageCase").focus();
        $("#storageCase").val("");
        $("#msg").html("");
        $("#PreBulkPackNoScanList").html("");
        $("#PreBulkPackDetailList").html("");
        $("#txt_SkuCount").html("0");
        $("#txt_SkuQtyCount").html("0");
        $("#qty").val("1");
    });

    function getPreBulkPackDetails(isRefresh) {
        $("#PreBulkPackNoScanList").html("");
        $("#PreBulkPackDetailList").html("");
        $("#txt_SkuCount").html("0");
        $("#txt_SkuQtyCount").html("0");
        com.CommonGetAjax("api/RFOutbound/GetPreBulkPackDetailsByStorageCase?storageCase=" + $("#storageCase").val().trim() + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
            if (data.RFCommResult.IsSucess) {
                if (isRefresh && data.PreBulkPackNoScan.length == 0) {
                    $("#tabLink1").removeClass("active");
                    $("#tab1").removeClass("active");
                    $("#tabLink2").addClass("active");
                    $("#tab2").addClass("active");
                } else {
                    $("#tabLink1").addClass("active");
                    $("#tab1").addClass("active");
                    $("#tabLink2").removeClass("active");
                    $("#tab2").removeClass("active");
                }

                if (data.PreBulkPackNoScan.length > 0) {
                    var jsonArray = new Array();
                    var skuQtyCount = 0;
                    for (var i = 0; i < data.PreBulkPackNoScan.length; i++) {
                        jsonArray.push('<tr>');
                        jsonArray.push('<td>' + (i + 1) + '</td>');
                        jsonArray.push('<td>' + data.PreBulkPackNoScan[i].UPC + '</td>');
                        jsonArray.push('<td>' + data.PreBulkPackNoScan[i].SkuName + '</td>');
                        if (data.PreBulkPackNoScan[i].PreQty == undefined || data.PreBulkPackNoScan[i].PreQty == null) {
                            jsonArray.push('<td></td>');
                        } else {
                            var noScanQty = parseInt(data.PreBulkPackNoScan[i].PreQty) - parseInt(data.PreBulkPackNoScan[i].Qty);
                            jsonArray.push('<td>' + noScanQty + '</td>');
                        }
                        jsonArray.push('<td>' + data.PreBulkPackNoScan[i].UOMCode + '</td>');
                        skuQtyCount += data.PreBulkPackNoScan[i].Qty;
                    }
                    $("#PreBulkPackNoScanList").append(jsonArray.join(''));
                } else {
                    com.NoData("#PreBulkPackNoScanList");
                }

                if (data.PreBulkPackDetails.length > 0) {
                    var jsonArray = new Array();
                    var skuQtyCount = 0;
                    for (var i = 0; i < data.PreBulkPackDetails.length; i++) {
                        jsonArray.push('<tr>');
                        jsonArray.push('<td>' + (i + 1) + '</td>');
                        jsonArray.push('<td>' + data.PreBulkPackDetails[i].UPC + '</td>');
                        jsonArray.push('<td>' + data.PreBulkPackDetails[i].SkuName + '</td>');
                        if (data.PreBulkPackDetails[i].PreQty == undefined || data.PreBulkPackDetails[i].PreQty == null) {
                            jsonArray.push('<td></td>');
                        } else {
                            jsonArray.push('<td>' + data.PreBulkPackDetails[i].PreQty + '</td>');
                        }
                        jsonArray.push('<td>' + data.PreBulkPackDetails[i].Qty + '</td>');
                        jsonArray.push('<td>' + data.PreBulkPackDetails[i].UOMCode + '</td>');
                        jsonArray.push('<td style="display:none">' + data.PreBulkPackDetails[i].SkuSysId + '</td></tr> ');
                        skuQtyCount += data.PreBulkPackDetails[i].Qty;
                    }
                    $("#PreBulkPackDetailList").append(jsonArray.join(''));
                } else {
                    com.NoData("#PreBulkPackDetailList");
                }
            } else {
                com.FailMsg("#msg", data.RFCommResult.Message);
                $("#storageCase").focus();
                $("#storageCase").val("");
                $("#skuUPC").val("");
                $("#qty").val("1"); $("#loading").remove();
            }
        });
        $("#loading").remove();
    };
});

//快速发货
$(document).on("pageInit", "#page-quickdelivery-list", function (e, pageId, $page) {
    $("#outboundOrder").focus();
    $("#outboundOrder").val("");
    $("#outboundSysId").val("");
    $("#OutboundDetailList").html("");
    $("#msg").html("");

    //扫描后检查出库单号是否存在
    $("#outboundOrder").keydown(function () {
        if (event.keyCode == 13) {
            $("#OutboundDetailList").html("");
            var outboundOrder = $("#outboundOrder").val();
            com.CommonAjax("api/RFOutbound/CheckOutboundExists", { OutboundOrder: outboundOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
                if (data.IsSucess) {
                    $("#msg").html("");
                    if (outboundOrder.trim() != "") {
                        com.CommonGetAjax("api/RFOutbound/GetOutboundDetailList?outboundOrder=" + outboundOrder + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
                            if (data != null && data.length > 0) {
                                var jsonArray = new Array();
                                for (var i = 0; i < data.length; i++) {
                                    jsonArray.push('<tr>');
                                    jsonArray.push('<td>' + (i + 1) + '</td>');
                                    jsonArray.push('<td>' + data[i].UPC + '</td>');
                                    jsonArray.push('<td>' + data[i].SkuName + '</td>');
                                    jsonArray.push('<td>' + data[i].SkuQty + '</td>');
                                    jsonArray.push('<td>' + data[i].UOMCode + '</td></tr> ');
                                    $("#outboundSysId").val(data[i].OutboundSysId);
                                }
                                $("#OutboundDetailList").append(jsonArray.join(''));
                            } else {
                                com.NoData("#OutboundDetailList");
                                $("#outboundSysId").val("");
                            }
                        });
                    }
                } else {
                    com.FailMsg("#msg", data.Message);
                    $("#outboundOrder").focus();
                    $("#outboundOrder").val("");
                    $("#outboundSysId").val("");
                }
            });
        }
    });

    //快速发货
    $("#btnQuickDelivery").on("click", function () {
        if ($("#outboundOrder").val().trim() != "" && $("#OutboundDetailList").children("tr").length > 0) {
            com.CommonAjax("api/MQ/OrderManagement/MQOutbound/OutboundQuickDelivery",
                {
                    SysId: $("#outboundSysId").val(),
                    OutboundOrder: $("#outboundOrder").val(),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                    CurrentUserId: GetlocalStorage("UserId"),
                    CurrentDisplayName: GetlocalStorage("DisplayName")
                }, function (data) {
                    com.SuccessMsg("#msg", "快速发货命令已提交，请等待处理");
                });
        } else {
            com.FailMsg("#msg", "请扫描单据条码");
            $("#outboundOrder").focus();
        }
    });

    //清空
    $("#btnReset").on("click", function () {
        $("#outboundOrder").focus();
        $("#outboundOrder").val("");
        $("#outboundSysId").val("");
        $("#OutboundDetailList").html("");
        $("#msg").html("");
    });
});

//收货
$(document).on("pageInit", "#page-receipt-list", function (e, pageId, $page) {
    sku.OnClose();
    $("#order").focus();
    $("#msg").html("");
    $("#receiptList").html("");

    $("#order").keydown(function () {
        if (event.keyCode == 13) {
            var receiptOrder = $("#order").val();
            com.CommonNoLoadingAjax("api/RFReceipt/CheckReceipt", {
                WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                CurrentUserId: GetlocalStorage("UserId"),
                ReceiptOrder: receiptOrder
            }, function (data) {
                if (data.IsSucess) {
                    $("#msg").html("");
                    if (receiptOrder.trim() != "") {
                        $.router.load("ReceiptDetail.html?receiptOrder=" + receiptOrder);
                    }
                } else {
                    com.FailMsg("#msg", data.Message);
                    $("#order").val("");
                    $("#order").focus();
                }
            });
        }
    });

    function getWaitingReceiptList() {
        loading = true;
        com.CommonLoadDataAjax("api/RFReceipt/GetWaitingReceiptListByPaging", {
            CurrentUserId: GetlocalStorage("UserId"),
            WarehouseSysId: GetlocalStorage("WarehouseSysId"),
            iDisplayStart: iDisplayStart,
            iDisplayLength: iDisplayLength
        }, function (data) {
            if (data.TableResuls.aaData != null && data.TableResuls.aaData.length > 0) {
                var jsonArray = new Array();
                for (var i = 0; i < data.TableResuls.aaData.length; i++) {
                    jsonArray.push('<tr onclick="$.router.load(\'ReceiptDetail.html?receiptOrder=' + data.TableResuls.aaData[i].ReceiptOrder + '\');">');
                    jsonArray.push('<td>' + (iDisplayStart + (i + 1)) + '</td>');
                    jsonArray.push('<td>' + data.TableResuls.aaData[i].ReceiptOrder + '</td>');
                    jsonArray.push('<td>' + data.TableResuls.aaData[i].DisplayStatus + '</td></tr> ');
                }
                $("#receiptList").append(jsonArray.join(''));
                iTotalDisplayRecords = data.TableResuls.iTotalDisplayRecords;
                loading = false;
                com.NoScroll();
            } else {
                if (data.TableResuls.iTotalDisplayRecords == 0) {
                    com.NoData("#receiptList");
                }
            }
        });
    }

    com.ScrollLoadData(getWaitingReceiptList);
    getWaitingReceiptList();
});

//收货明细
$(document).on("pageInit", "#page-receipt-detail", function (e, pageId, $page) {
    $("#receiptOperationDetailList").html("");
    $("#msg").html("");
    $("#skuCount").html("0");
    $("#qty").val("");
    $("#skuUPC").val("");
    $("#skuUPC").focus();
    var receiptOrder = com.GetQueryString("receiptOrder");
    var scanSkus = new Array();

    getReceiptOperationDetailList();

    //获取入库单收货明细
    function getReceiptOperationDetailList() {
        com.CommonAjax("api/RFReceipt/GetReceiptOperationDetailList", {
            ReceiptOrder: receiptOrder,
            WarehouseSysId: GetlocalStorage("WarehouseSysId"),
            CurrentUserId: GetlocalStorage("UserId")
        }, function (data) {
            if (data != null && data.length > 0) {
                sku.SetSkuDetailList(data);
                var jsonArray = new Array();
                for (var i = 0; i < data.length; i++) {
                    jsonArray.push('<tr>');
                    jsonArray.push('<td>' + (i + 1) + '</td>');
                    jsonArray.push('<td style="display:none">' + data[i].SkuSysId + '</td>');
                    jsonArray.push('<td>' + ((data[i].UPC == undefined || data[i].UPC == null) ? "" : data[i].UPC) + '</td>');
                    jsonArray.push('<td>' + data[i].SkuName + '</td>');
                    jsonArray.push('<td>' + data[i].PurchaseQty + '</td>');
                    jsonArray.push('<td>' + data[i].ReceiptQty + '</td>');
                    jsonArray.push('<td>' + data[i].RejectQty + '</td></tr>');
                }
                $("#receiptOperationDetailList").append(jsonArray.join(''));
            } else {
                com.NoData("#receiptOperationDetailList");
            }
        });
    }

    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });

    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            if (!sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#qty", null)) {
                com.FailMsg("#msg", "扫描条码在此单据不存在");
                $("#skuUPC").val("");
                $("#skuUPC").focus();
                return false;
            }
            $("#msg").html("");
            $("#qty").focus();
        }
    });

    $("#rejectQty").on("keyup", function () {
        var testRegex = /^\d+$/;
        if (!testRegex.test($("#rejectQty").val())) {
            $("#rejectQty").val("");
            $("#rejectQty").focus();
            com.FailMsg("#msg", "普通商品拒收数量必须为整数");
        }
    });

    $("#rejectQty").on("keydown", function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });

    $("#qty").on("keyup", function () {
        var testRegex = /^\d+$/;
        if ($("#qty").val() != "" && !testRegex.test($("#qty").val())) {
            $("#qty").val("");
            $("#qty").focus();
            com.FailMsg("#msg", "普通商品收货数量必须为整数");
        }
    });

    $("#qty").on("keydown", function () {
        $("#msg").html("");
        if (event.keyCode == 13) {
            var skuUPC = $("#skuUPC").val().trim;
            var rejectQty = $("#rejectQty").val().trim();
            if (rejectQty == "") { rejectQty = 0 };
            var qty = $("#qty").val().trim();

            if (skuUPC == "") {
                com.FailMsg("#msg", "商品条码不能为空");
                $("#skuUPC").focus();
                return false;
            }
            if (qty == "") {
                com.FailMsg("#msg", "商品数量不能为空");
                $("#qty").focus();
                return false;
            }

            var inputQty = parseInt(qty);
            var inputRejectQty = parseInt(rejectQty);
            var isQtyExceeded = false;
            $("#receiptOperationDetailList").find("tr").each(function () {
                var tds = $(this).children();
                var rowSkuSysId = tds.eq(1).text();
                var rowPurchaseQty = parseInt(tds.eq(4).text());
                var rowReceiptQty = parseInt(tds.eq(5).text());
                var rowRejectQty = parseInt(tds.eq(6).text());
                if (sku.GetSkuSysId() == rowSkuSysId) {
                    if (rowPurchaseQty < (rowReceiptQty + rowRejectQty + inputQty + inputRejectQty)) {
                        isQtyExceeded = true;
                    } else {
                        tds.eq(5).text(rowReceiptQty + inputQty);
                        tds.eq(6).text(rowRejectQty + inputRejectQty);
                    }
                    return false;
                }
            });

            if (isQtyExceeded == true) {
                com.FailMsg("#msg", "超出入库数量，无法继续收货");
            } else {
                $("#skuUPC").val("");
                $("#rejectQty").val("");
                $("#qty").val("");
                $("#skuUPC").focus();

                var upcFlag = $("#selectkey").val().split('|')[0];
                if (upcFlag == "S" || upcFlag == "P") {
                    scanSkus.push($("#selectkey").val());
                    $("#skuCount").text(getCount(scanSkus));
                }
            }
        }
    });

    $("#btnFinish").on("click", function () {
        $.showIndicator();
        var lotTemplateValueDtos = [];
        var receiptDetailOperationDto = [];

        $("#receiptOperationDetailList").find("tr").each(function () {
            var tds = $(this).children();
            var skuSysId = tds.eq(1).text();
            var receiptQty = parseInt(tds.eq(5).text());
            var rejectedQty = parseInt(tds.eq(6).text());
            if (receiptQty > 0 || rejectedQty > 0) {
                lotTemplateValueDtos.push({ SkuSysId: skuSysId, Qty: receiptQty });
            }
            receiptDetailOperationDto.push({ SkuSysId: skuSysId, ReceivedQty: receiptQty, RejectedQty: rejectedQty, RejectedGiftQty: 0, GiftQty: 0, Descr: "" });
        });

        if (lotTemplateValueDtos.length == 0) {
            com.FailMsg("#msg", "请扫描收货明细");
            $("#skuUPC").focus();
            return false;
        }

        com.CommonNoLoadingAjax("api/RFReceipt/SaveReceiptOperation", {
            ReceiptOrder: receiptOrder,
            LotTemplateValueDtos: lotTemplateValueDtos,
            ReceiptDetailOperationDto: receiptDetailOperationDto,
            CurrentUserId: GetlocalStorage("UserId"),
            CurrentDisplayName: GetlocalStorage("DisplayName"),
            WarehouseSysId: GetlocalStorage("WarehouseSysId")
        }, function (data) {
            $.hideIndicator();
            if (data != null) {
                if (data.IsSucess) {
                    $.alert('收货完成', function () {
                        $.router.load("../../Home.html");
                    });
                } else {
                    com.FailMsg("#msg", data.Message);
                }
            }
        });
    });

    $("#txt_DetailList").on("click", function () {
        if ($("#icon_DetailList").hasClass("fa fa-chevron-down")) {
            $("#icon_DetailList").attr("class", "fa fa-chevron-up");
            $("#div_DetailList").show("normal");
        } else {
            $("#icon_DetailList").attr("class", "fa fa-chevron-down");
            $("#div_DetailList").hide("normal");
        }
    });

    function getCount(arr) {
        var n = [];
        for (var i = 0; i < arr.length; i++) {
            if (n.indexOf(arr[i]) == -1) n.push(arr[i]);
        }
        return n.length;
    }
});

//库位变更
$(document).on("pageInit", "#page-stockmovement-index", function (e, pageId, $page) {
    $("#fromLoc").focus();
    $("#msg").html("");

    //来源货位
    $("#fromLoc").on("keydown", function () {
        if (event.keyCode == 13) {
            $("#fromLot").focus();
        }
    });
    $("#fromLoc").on("blur", function () {
        if ($("#fromLoc").val().trim() != "") {
            com.CommonAjax("api/Base/LocIsExist",
                {
                    LocationSearch: $("#fromLoc").val(),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    if (data != null) {
                        if (!data.IsSucess) {
                            com.FailMsg("#msg", data.Message);
                            $("#fromLoc").focus();
                            $("#fromLoc").val("");
                        } else {
                            $("#msg").html("");
                        }
                    }
                });
        }
    });

    //来源批次
    $("#fromLot").on("keydown", function () {
        if (event.keyCode == 13) {
            $("#skuUPC").focus();
        }
    });

    //UPC
    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#toLoc").focus();
        }
    });
    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            com.CommonNoLoadingGetAjax("api/Base/GetSkuPackListByUPC?upc=" + $("#skuUPC").val() + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
                if (data != null) {
                    sku.SetSkuDetailList(data);

                    if (!sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#toLoc", null, null)) {
                        com.FailMsg("#msg", "商品不存在");
                        $("#skuUPC").focus();
                        $("#skuUPC").val("");
                        return false;
                    } else {
                        $("#msg").html("");
                        $("#toLoc").focus();
                    }
                } else {
                    com.FailMsg("#msg", "商品不存在");
                    return false;
                }
            });
        }
    });

    //目标货位
    $("#toLoc").on("keydown", function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });
    $("#toLoc").on("blur", function () {
        if ($("#toLoc").val().trim() != "") {
            com.CommonAjax("api/Base/LocIsExist",
                {
                    LocationSearch: $("#toLoc").val(),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    if (data != null) {
                        if (!data.IsSucess) {
                            com.FailMsg("#msg", data.Message);
                            $("#toLoc").focus();
                            $("#toLoc").val("");
                        } else {
                            $("#msg").html("");
                        }
                    }
                });
        }
    });

    //上架数量必须为数字
    $("#qty").on("keyup", function () {
        if (!com.CheckNumber($("#qty").val())) {
            $("#qty").val("");
        }
    });
    $("#qty").on("keydown", function () {
        $("#msg").html("");
        if (event.keyCode == 13) {
            var fromLoc = $("#fromLoc").val();
            var fromLot = $("#fromLot").val();
            var toLoc = $("#toLoc").val();
            var upc = $("#skuUPC").val();
            var qty = $("#qty").val();
            var skuSysId = sku.GetSkuSysId();

            if (fromLoc.trim() == "") {
                com.FailMsg("#msg", "来源货位不能为空");
                $("#fromLoc").focus();
                return false;
            }
            if (upc.trim() == "") {
                com.FailMsg("#msg", "商品条码不能为空");
                $("#skuUPC").focus();
                return false;
            }
            if (toLoc.trim() == "") {
                com.FailMsg("#msg", "目标货位不能为空");
                $("#toLoc").focus();
                return false;
            }
            if (qty.trim() == "") {
                com.FailMsg("#msg", "商品数量不能为空");
                $("#qty").focus();
                return false;
            }
            if (parseFloat(qty.trim()) <= 0) {
                com.FailMsg("#msg", "商品数量必须大于0");
                $("#qty").focus();
                return false;
            }
            $.showIndicator();
            com.CommonAjax("api/RFInventory/StockMovement",
                {
                    SkuSysId: skuSysId,
                    UPC: upc,
                    FromLoc: fromLoc,
                    FromLot: fromLot,
                    ToLoc: toLoc,
                    InputQty: qty,
                    CurrentUserId: GetlocalStorage("UserId"),
                    CurrentDisplayName: GetlocalStorage("DisplayName"),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    $.hideIndicator();
                    if (data != null) {
                        if (data.IsSucess) {
                            com.SuccessMsg("#msg", data.Message);
                            $("#fromLoc").val("");
                            $("#fromLot").val("");
                            $("#skuUPC").val("");
                            $("#toLoc").val("");
                            $("#qty").val("");
                            $("#fromLoc").focus();
                        } else {
                            com.FailMsg("#msg", data.Message);
                        }
                    }
                });
        }
    });

});

//容器拣货
$(document).on("pageInit", "#page-containerpicking-list", function (e, pageId, $page) {
    sku.OnClose();
    $("#order").focus();
    $("#msg").html("");
    $("#pickingList").html("");

    $("#order").keydown(function () {
        if (event.keyCode == 13) {
            var outboundOrder = $("#order").val();
            com.CommonNoLoadingAjax("api/PickDetail/CheckContainerPickingOutboundOrder", { OutboundOrder: outboundOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
                if (data.IsSucess) {
                    $("#msg").html("");
                    if (outboundOrder.trim() != "") {
                        $.router.load("ContainerPickingDetail.html?outboundOrder=" + data.Message);
                    }
                } else {
                    com.FailMsg("#msg", data.Message);
                    $("#order").val("");
                    $("#order").focus();
                }
            });
        }
    });

    function getWaitingPickList() {
        loading = true;
        com.CommonLoadDataAjax("api/PickDetail/GetWaitingContainerPickingListByPaging", {
            CurrentUserId: GetlocalStorage("UserId"),
            WarehouseSysId: GetlocalStorage("WarehouseSysId"),
            iDisplayStart: iDisplayStart,
            iDisplayLength: iDisplayLength
        }, function (data) {
            if (data.TableResuls.aaData != null && data.TableResuls.aaData.length > 0) {
                var jsonArray = new Array();
                for (var i = 0; i < data.TableResuls.aaData.length; i++) {
                    jsonArray.push('<tr onclick="$.router.load(\'ContainerPickingDetail.html?outboundOrder=' + data.TableResuls.aaData[i].OutboundOrder + '\');">');
                    jsonArray.push('<td>' + (iDisplayStart + (i + 1)) + '</td>');
                    jsonArray.push('<td>' + data.TableResuls.aaData[i].OutboundOrder + '</td>');
                    jsonArray.push('<td>' + data.TableResuls.aaData[i].TotalQty + '</td>');
                    jsonArray.push('<td>' + data.TableResuls.aaData[i].SkuQty + '</td></tr> ');
                }
                $("#pickingList").append(jsonArray.join(''));
                iTotalDisplayRecords = data.TableResuls.iTotalDisplayRecords;
                loading = false;
                com.NoScroll();
            } else {
                if (data.TableResuls.iTotalDisplayRecords == 0) {
                    com.NoData("#pickingList");
                }
            }
        });
    }

    com.ScrollLoadData(getWaitingPickList);
    getWaitingPickList();
});

//容器拣货明细
$(document).on("pageInit", "#page-containerpicking-detail", function (e, pageId, $page) {
    $("#containerPickingDetailList").html("");
    $("#msg").html("");
    $("#skuCount").html("0");
    $("#qty").val("");
    $("#container").val("");
    $("#container").focus();
    var outboundOrder = com.GetQueryString("outboundOrder");
    var scanSkus = new Array();

    getContainerPickingDetailList();

    //获取容器拣货明细
    function getContainerPickingDetailList() {
        com.CommonAjax("api/PickDetail/GetContainerPickingDetailList", { OutboundOrder: outboundOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
            if (data != null && data.length > 0) {
                sku.SetSkuDetailList(data);
                var jsonArray = new Array();
                for (var i = 0; i < data.length; i++) {
                    jsonArray.push('<tr>');
                    jsonArray.push('<td>' + (i + 1) + '</td>');
                    jsonArray.push('<td style="display:none">' + data[i].SkuSysId + '</td>');
                    jsonArray.push('<td>' + ((data[i].UPC == undefined || data[i].UPC == null) ? "" : data[i].UPC) + '</td>');
                    jsonArray.push('<td>' + data[i].SkuName + '</td>');
                    jsonArray.push('<td>' + data[i].Loc + '</td>');
                    jsonArray.push('<td>' + data[i].Qty + '</td>');
                    jsonArray.push('<td>' + data[i].PickedQty + '</td></tr>');
                }
                $("#containerPickingDetailList").append(jsonArray.join(''));
            } else {
                com.NoData("#containerPickingDetailList");
            }
        });
    }

    $("#container").keydown(function () {
        var container = $("#container").val().trim();
        if (event.keyCode == 13) {
            if (container != "") {
                com.CommonNoLoadingGetAjax("api/PickDetail/CheckContainerIsAvailable?storageCase=" + container + "&outboundOrder=" + outboundOrder + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
                    if (data.IsSucess) {
                        $("#msg").html("");
                        $("#skuUPC").focus();
                    } else {
                        com.FailMsg("#msg", data.Message);
                        $("#container").focus();
                    }
                });
            } else {
                $("#msg").html("");
                $("#skuUPC").focus();
            }
        }
    });

    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#loc").focus();
        }
    });

    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            if (!sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#qty", null, function () {
                $("#msg").html("");
                $("#loc").focus();
            })) {
                com.FailMsg("#msg", "扫描条码在此单据不存在");
                $("#skuUPC").val("");
                $("#skuUPC").focus();
                return false;
            }
            $("#msg").html("");
            $("#loc").focus();
        }
    });

    $("#loc").keydown(function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });

    //$("#loc").on("blur", function () {
    //    if ($("#loc").val().trim() == "") {
    //        com.FailMsg("#msg", "请扫描库位条码");
    //    }
    //});

    $("#qty").on("keyup", function () {
        if (!com.CheckPositiveNumber($("#qty").val())) {
            $("#qty").val("");
        }
    });

    $("#qty").on("keydown", function () {
        $("#msg").html("");
        if (event.keyCode == 13) {
            var container = $("#container").val().trim();
            var skuUPC = $("#skuUPC").val().trim();
            var loc = $("#loc").val().trim();
            var qty = $("#qty").val().trim();

            if (skuUPC == "") {
                com.FailMsg("#msg", "商品条码不能为空");
                $("#skuUPC").focus();
                return false;
            }
            if (loc == "") {
                com.FailMsg("#loc", "库位条码不能为空");
                $("#skuUPC").focus();
                return false;
            }
            if (qty == "") {
                com.FailMsg("#msg", "拣货数量不能为空");
                $("#qty").focus();
                return false;
            }

            if (qty <= 0) {
                com.FailMsg("#msg", "拣货数量不能小于等于0");
                $("#qty").focus();
                return false;
            }

            var inputQty = parseInt(qty);
            var isQtyExceeded = false;
            $("#containerPickingDetailList").find("tr").each(function () {
                var tds = $(this).children();
                var rowSkuSysId = tds.eq(1).text();
                var rowLoc = tds.eq(4).text();
                var rowQty = parseInt(tds.eq(5).text());
                var rowPickedQty = parseInt(tds.eq(6).text());
                if (sku.GetSkuSysId() == rowSkuSysId && loc == rowLoc) {
                    if (rowQty < (rowPickedQty + inputQty)) {
                        isQtyExceeded = true;
                    }
                    return false;
                }
            });

            if (isQtyExceeded == true) {
                com.FailMsg("#msg", "超出分配数量，无法继续拣货");
            } else {
                com.CommonNoLoadingAjax("api/PickDetail/GenerateContainerPickingDetail", {
                    OutboundOrder: outboundOrder,
                    StorageCase: container,
                    UPC: skuUPC,
                    SkuSysId: sku.GetSkuSysId(),
                    Loc: loc,
                    PickedQty: inputQty,
                    CurrentUserId: GetlocalStorage("UserId"),
                    CurrentDisplayName: GetlocalStorage("DisplayName"),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                }, function (data) {
                    $.hideIndicator();
                    if (data != null) {
                        if (data.IsSucess) {
                            $("#containerPickingDetailList").find("tr").each(function () {
                                var tds = $(this).children();
                                var rowSkuSysId = tds.eq(1).text();
                                var rowLoc = tds.eq(4).text();
                                var rowPickedQty = parseInt(tds.eq(6).text());
                                if (sku.GetSkuSysId() == rowSkuSysId && loc == rowLoc) {
                                    tds.eq(6).text(rowPickedQty + inputQty);
                                    return false;
                                }
                            });
                            com.SuccessMsg("#msg", '拣货成功！商品：' + skuUPC + ', 库位：' + loc + ', 数量：' + inputQty);

                            var upcFlag = $("#selectkey").val().split('|')[0];
                            if (upcFlag == "S" || upcFlag == "P") {
                                scanSkus.push($("#selectkey").val());
                                $("#skuCount").text(getCount(scanSkus));
                            }

                            $("#skuUPC").val("");
                            $("#loc").val("");
                            $("#qty").val("");
                            $("#skuUPC").focus();

                        } else {
                            com.FailMsg("#msg", data.Message);
                        }
                    }
                });
            }
        }
    });

    $("#txt_DetailList").on("click", function () {
        if ($("#icon_DetailList").hasClass("fa fa-chevron-down")) {
            $("#icon_DetailList").attr("class", "fa fa-chevron-up");
            $("#div_DetailList").show("normal");
            $("#container").focus();
        } else {
            $("#icon_DetailList").attr("class", "fa fa-chevron-down");
            $("#div_DetailList").hide("normal");
            $("#container").focus();
        }
    });

    function getCount(arr) {
        var n = [];
        for (var i = 0; i < arr.length; i++) {
            if (n.indexOf(arr[i]) == -1) n.push(arr[i]);
        }
        return n.length;
    }
});

//交接箱复核
$(document).on("pageInit", "#page-transferorderreview-detail", function (e, pageId, $page) {
    $("#loading").remove();
    $("#storageCase").focus();
    $("#msg").html("");
    $("#TransferOrderReviewList").html("");

    $("#storageCase").keydown(function () {
        if (event.keyCode == 13) {
            $("#msg").html("");
            if ($("#storageCase").val().trim() != "") {
                getPreBulkPackDetails(false);
                $("#transferOrder").focus();
            } else {
                com.FailMsg("#msg", "请扫描容器");
                //$("#msg").html("请扫描容器");
            }
        }
    });

    $("#transferOrder").keydown(function () {
        if (event.keyCode == 13) {
            $("#msg").html("");
            if ($("#transferOrder").val().trim() != "") {
                //此处需要查询交接数据
                checkTransferOrder(false);
                $("#skuUPC").focus();
            } else {
                com.FailMsg("#msg", "请扫描交接箱条码");
                // $("#msg").html("请扫描交接箱条码");
            }
        }
    });

    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });

    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            if (!sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#qty", null)) {
                com.FailMsg("#msg", "扫描条码在此单据不存在");
                $("#skuUPC").focus();
                $("#skuUPC").val("");
                return false;
            } else {
                $("#msg").html("");
                $("#qty").focus();
            }
        }
    });

    $("#qty").keydown(function () {
        if (event.keyCode == 13) {

            var storageCase = $("#storageCase").val().trim();
            var transferOrder = $("#transferOrder").val().trim();
            var upc = $("#skuUPC").val().trim();
            var qty = $("#qty").val().trim();
            var skuSysId = sku.GetSkuSysId();

            if (storageCase == "") {
                com.FailMsg("#msg", "容器号不能为空");
                $("#storageCase").focus();
                return false;
            }
            if (transferOrder == "") {
                com.FailMsg("#msg", "交接箱号不能为空");
                $("#toLoc").focus();
                return false;
            }
            if (upc == "") {
                com.FailMsg("#msg", "商品条码不能为空");
                $("#skuUPC").focus();
                return false;
            }
            if (qty == "") {
                com.FailMsg("#msg", "商品数量不能为空");
                $("#qty").focus();
                return false;
            }
            if (parseFloat(qty) <= 0) {
                com.FailMsg("#msg", "商品数量必须大于0");
                $("#qty").focus();
                return false;
            }

            com.CommonAjax("api/RFOutbound/AddOutboundTransferOrder",
                {
                    StorageCase: storageCase,
                    TransferOrder: transferOrder,
                    SkuSysId: skuSysId,
                    UPC: $("#skuUPC").val(),
                    Qty: qty,
                    CurrentUserId: GetlocalStorage("UserId"),
                    CurrentDisplayName: GetlocalStorage("DisplayName"),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                },
                function (data) {
                    if (data != null) {
                        if (!data.IsSucess) {
                            $("#qty").focus();
                            $("#qty").val("");
                            com.FailMsg("#msg", data.Message);
                        } else {
                            com.SuccessMsg("#msg", '复核成功');

                            $("#TransferOrderReviewList").find("tr").each(function () {
                                var tds = $(this).children();
                                if (tds.eq(4).text() == skuSysId) {
                                    var number = tds.eq(3).text();
                                    tds.eq(3).text(parseInt(number) - parseInt(qty));
                                    return false;
                                }
                            });

                            $("#skuUPC").val("");
                            $("#skuUPC").focus();
                            $("#qty").val("");
                        }
                    }
                });
        }
    });

    //封箱
    $("#btnFinish").on('click', function () {
        var transferOrder = $("#transferOrder").val().trim();
        if (transferOrder == '') {
            com.FailMsg("#msg", "交接条码不能为空");
            // $("#msg").html("交接条码不能为空");
            return false;
        }
        com.CommonNoLoadingAjax("api/RFOutbound/SealedOutboundTransferBox",
               {
                   TransferOrder: transferOrder,
                   CurrentUserId: GetlocalStorage("UserId"),
                   CurrentDisplayName: GetlocalStorage("DisplayName"),
                   WarehouseSysId: GetlocalStorage("WarehouseSysId")
               },
               function (data) {
                   if (data != null) {
                       if (!data.IsSucess) {
                           com.FailMsg("#msg", data.Message);
                       } else {
                           com.SuccessMsg("#msg", data.Message);
                           $("#storageCase").val("");
                           $("#transferOrder").val("");
                           $("#skuUPC").val();
                           $("#qty").val("");
                           $("#storageCase").focus();
                       }
                   }
               });
    });

    //检查交接单是否是次出库单
    function checkTransferOrder(isRefresh) {
        if ($("#storageCase").val().trim() == "") {
            com.FailMsg("#msg", "容器条码不能为空");
            // $("#msg").html("容器条码不能为空");
            $("#storageCase").focus();
            return false;
        }
        com.CommonGetAjax("api/RFOutbound/CheckTransferOrderAndPreBulkPack?transferOrder=" + $("#transferOrder").val().trim() + "&storageCase=" + $("#storageCase").val().trim() + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
            if (!data.IsSucess) {
                $("#transferOrder").val("");
                $("#transferOrder").focus();
                com.FailMsg("#msg", data.Message);
            } else {
                $("#outboundOrderNumber").val(data.Message);
            }
        });
    };

    //容器数据展示
    function getPreBulkPackDetails(isRefresh) {
        $("#TransferOrderReviewList").html("");
        com.CommonGetAjax("api/RFOutbound/GetStorageCaseSkuList?storageCase=" + $("#storageCase").val().trim() + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
            if (data != null && data.length > 0) {
                sku.SetSkuDetailList(data);
                var jsonArray = new Array();
                for (var i = 0; i < data.length; i++) {
                    jsonArray.push('<tr>');
                    jsonArray.push('<td>' + (i + 1) + '</td>');
                    jsonArray.push('<td>' + data[i].UPC + '</td>');
                    jsonArray.push('<td>' + data[i].SkuName + '</td>');
                    jsonArray.push('<td>' + data[i].Qty + '</td>');
                    jsonArray.push('<td style="display:none">' + data[i].SkuSysId + '</td></tr> ');
                }
                $("#TransferOrderReviewList").append(jsonArray.join(''));

            } else {
                com.FailMsg("#msg", "容器不存在或容器数据已经为空");
                $("#storageCase").focus();
                $("#storageCase").val("");
                $("#transferOrder").val("");
                $("#skuUPC").val("");
                $("#qty").val("");
                $("#loading").remove();
            }
        });
        $("#loading").remove();
    };

    $("#lookDifference").on('click', function () {

        var storageCase = $("#storageCase").val().trim();
        var transferOrder = $("#transferOrder").val().trim();
        if (storageCase == "") {
            com.FailMsg("#msg", "容器号不能为空");
            $("#storageCase").focus();
            return false;
        }
        if (transferOrder == "") {
            com.FailMsg("#msg", "交接箱号不能为空");
            $("#transferOrder").focus();
            return false;
        }
        var outboundOrder = $("#outboundOrderNumber").val().trim();
        $.router.load("TransferOrderReviewDiff.html?outboundOrder=" + outboundOrder);
    });

});

//查看交接箱复核差异
$(document).on("pageInit", "#page-transferorderreviewdiff-detail", function (e, pageId, $page) {
    $("#TransferOrderReviewDiffList").empty();
    var outboundOrder = com.GetQueryString("outboundOrder");
    com.CommonAjax("api/RFOutbound/GetTransferOrderReviewDiffList",
        {
            OutboundOrder: outboundOrder,
            WarehouseSysId: GetlocalStorage("WarehouseSysId")
        }, function (data) {
            if (data.length > 0) {
                var jsonArray = new Array();
                for (var i = 0; i < data.length; i++) {
                    jsonArray.push('<tr>');
                    jsonArray.push('<td>' + (i + 1) + '</td>');
                    jsonArray.push('<td>' + data[i].UPC + '</td>');
                    jsonArray.push('<td>' + data[i].SkuName + '</td>');
                    jsonArray.push('<td>' + data[i].OutboundQty + '</td>');
                    jsonArray.push('<td>' + data[i].ReviewQty + '</td>');
                    jsonArray.push('<td style="display:none">' + data[i].SkuSysId + '</td></tr> ');
                }
                $("#TransferOrderReviewDiffList").append(jsonArray.join(''));
            } else {
                com.NoData("#TransferOrderReviewDiffList");
            }
        });
});


//拣货
$(document).on("pageInit", "#page-picking-list", function (e, pageId, $page) {
    sku.OnClose();
    $("#order").focus();
    $("#msg").html("");
    $("#pickingList").html("");

    $("#order").keydown(function () {
        if (event.keyCode == 13) {
            var outboundOrder = $("#order").val();
            com.CommonNoLoadingAjax("api/PickDetail/CheckContainerPickingOutboundOrder", { OutboundOrder: outboundOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
                if (data.IsSucess) {
                    $("#msg").html("");
                    if (outboundOrder.trim() != "") {
                        $.router.load("PickingDetail.html?outboundOrder=" + data.Message);
                    }
                } else {
                    com.FailMsg("#msg", data.Message);
                    $("#order").val("");
                    $("#order").focus();
                }
            });
        }
    });

    function getWaitingPickList() {
        loading = true;
        com.CommonLoadDataAjax("api/PickDetail/GetWaitingContainerPickingListByPaging", {
            CurrentUserId: GetlocalStorage("UserId"),
            WarehouseSysId: GetlocalStorage("WarehouseSysId"),
            iDisplayStart: iDisplayStart,
            iDisplayLength: iDisplayLength
        }, function (data) {
            if (data.TableResuls.aaData != null && data.TableResuls.aaData.length > 0) {
                var jsonArray = new Array();
                for (var i = 0; i < data.TableResuls.aaData.length; i++) {
                    jsonArray.push('<tr onclick="$.router.load(\'PickingDetail.html?outboundOrder=' + data.TableResuls.aaData[i].OutboundOrder + '\');">');
                    jsonArray.push('<td>' + (iDisplayStart + (i + 1)) + '</td>');
                    jsonArray.push('<td>' + data.TableResuls.aaData[i].OutboundOrder + '</td>');
                    jsonArray.push('<td>' + data.TableResuls.aaData[i].TotalQty + '</td>');
                    jsonArray.push('<td>' + data.TableResuls.aaData[i].SkuQty + '</td></tr> ');
                }
                $("#pickingList").append(jsonArray.join(''));
                iTotalDisplayRecords = data.TableResuls.iTotalDisplayRecords;
                loading = false;
                com.NoScroll();
            } else {
                if (data.TableResuls.iTotalDisplayRecords == 0) {
                    com.NoData("#pickingList");
                }
            }
        });
    }

    com.ScrollLoadData(getWaitingPickList);
    getWaitingPickList();
});

//拣货明细
$(document).on("pageInit", "#page-picking-detail", function (e, pageId, $page) {
    $("#msg").html("");
    $("#skuCount").html("0");
    $("#qty").val("");
    $("#container").val("");
    $("#lot").val("");
    $("#loc").focus();
    var outboundOrder = com.GetQueryString("outboundOrder");
    var isDiff = com.GetQueryString("isDiff");
    var scanSkus = new Array();
    var index = 0;
    $("#currentLoc").css("background-color", "#eee");

    getContainerPickingDetailList();

    //获取容器拣货明细
    function getContainerPickingDetailList() {
        if (isDiff == "1") {
            loadSkuInfo();
        } else {
            sku.SetSkuDetailList(null);
            com.CommonAjax("api/PickDetail/GetContainerPickingDetailList", { OutboundOrder: outboundOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
                if (data != null && data.GroupedPickingDetails.length > 0) {
                    sku.SetSkuDetailList(data.GroupedPickingDetails);
                    picking.SetDetailList(data.PickingDetails);
                    loadSkuInfo();
                } else {
                    com.FailMsg("#msg", "不存在待拣货商品");
                }
            });
        }
    }

    function loadSkuInfo(currentIndex) {
        if (picking.DetailList != null && picking.DetailList.length > 0) {
            var startIndex = currentIndex != undefined ? currentIndex : 0;
            $("#currentUomName").text("");
            for (var i = startIndex; i < picking.DetailList.length; i++) {
                if (picking.DetailList[i].Qty != (parseInt(picking.DetailList[i].PickedQty) + parseInt(picking.DetailList[i].CurrentPickedQty))) {
                    if (sku.GetSkuSysId() != picking.DetailList[i].SkuSysId || (parseInt(picking.DetailList[i].PickedQty) + parseInt(picking.DetailList[i].CurrentPickedQty)) == 0) {
                        $("#currentLoc").css("background-color", "#eee");
                        $("#loc").val("");
                        $("#container").val("");
                        $("#lot").val("");
                        $("#loc").focus();
                    }
                    $("#currentLoc").val(picking.DetailList[i].Loc);
                    $("#currentSkuName").val(picking.DetailList[i].SkuName);
                    $("#currentSkuUPC").val(picking.DetailList[i].UPC);
                    $("#currentLot").val(picking.DetailList[i].Lot);
                    $("#currentQty").val(picking.DetailList[i].Qty);
                    $("#currentUomName").text(picking.DetailList[i].UomNameDisplay);
                    $("#currentSkuSysId").val(picking.DetailList[i].SkuSysId);

                    index = i;
                    break;
                }
            }

            var text = "拣货进度：" + (index + 1) + "/" + picking.DetailList.length;
            $("#btnProgress").html(text);

            var totalPickedQty = parseInt(picking.DetailList[index].PickedQty) + parseInt(picking.DetailList[index].CurrentPickedQty);
            var currentQty = parseInt($("#currentQty").val().trim());
            var qtyText = "拣货数量：" + totalPickedQty + "/" + currentQty;
            $("#btnQtyProgress").html(qtyText);
        } else {
            com.FailMsg("#msg", "不存在待拣货商品");
        }
    }

    $("#loc").keydown(function () {
        if (event.keyCode == 13) {
            if ($("#loc").val().trim() == $("#currentLoc").val().trim()) {
                $("#currentLoc").css("background-color", "#7ebb94");
                $("#container").focus();
                $("#msg").html("");
            } else {
                $("#currentLoc").css("background-color", "#eee");
                com.FailMsg("#msg", "请扫描正确的库位条码");
            }
        }
    });

    $("#container").keydown(function () {
        var container = $("#container").val().trim();
        if (event.keyCode == 13) {
            if (container != "") {
                com.CommonNoLoadingGetAjax("api/PickDetail/CheckContainerIsAvailable?storageCase=" + container + "&outboundOrder=" + outboundOrder + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
                    if (data.IsSucess) {
                        $("#msg").html("");
                        $("#lot").focus();
                    } else {
                        com.FailMsg("#msg", data.Message);
                        $("#container").focus();
                    }
                });
            } else {
                $("#msg").html("");
                $("#lot").focus();
            }
        }
    });

    $("#lot").keydown(function () {
        var lot = $("#lot").val().trim();
        if (event.keyCode == 13) {
            if (lot != "" && lot != $("#currentLot").val().trim()) {
                com.FailMsg("#msg", "请扫描正确的批次条码");
                $("#lot").focus();
            } else {
                $("#msg").html("");
                $("#skuUPC").focus();
            }
        }
    });

    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#msg").html("");
            $("#qty").val("");
            $("#qty").focus();
        }
    });

    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            if (!sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#qty",
            function () {
                $("#msg").html("");
            },
            function () {

                if (sku.GetSkuSysId() != $("#currentSkuSysId").val()) {
                    com.FailMsg("#msg", "扫描商品不是当前待拣货商品，请重新扫描");
                    $("#skuUPC").val("");
                    $("#skuUPC").focus();
                    return false;
            }

                if ($('#continuousMode').is(':checked')) {
                    var scanningQty = 1;
                    if ($("#qty").val().trim() != "") {
                        scanningQty = parseInt($("#qty").val().trim());
            }
                    skuScanning(scanningQty);
            }
            })) {
                com.FailMsg("#msg", "扫描商品不是当前待拣货商品，请重新扫描");
                $("#skuUPC").val("");
                $("#skuUPC").focus();
                return false;
            }
        }
        //else {
        //    com.FailMsg("#msg", "请扫描正确的商品条码");
        //    $("#skuUPC").val("");
        //    $("#skuUPC").focus();
        //    return false;
        //}
    });

    $("#qty").on("keyup", function () {
        if (!com.CheckPositiveNumber($("#qty").val())) {
            $("#qty").val("");
        }
    });

    $("#qty").on("keydown", function () {
        $("#msg").html("");
        if (event.keyCode == 13) {
            skuScanning(parseInt($("#qty").val().trim()));
        }
    });

    function skuScanning(scanningQty) {
        var currentSku = picking.DetailList[index];
        var loc = $("#loc").val().trim();
        var lot = $("#lot").val().trim();
        var container = $("#container").val().trim();
        var skuUPC = $("#skuUPC").val().trim();
        var qty = scanningQty;
        var currentQty = parseInt($("#currentQty").val().trim());

        if (loc == "") {
            com.FailMsg("#msg", "库位条码不能为空");
            $("#skuUPC").focus();
            return false;
        }
        if (lot != "" && lot.toUpperCase() != currentSku.Lot.toUpperCase()) {
            com.FailMsg("#msg", "请扫描正确的批次条码");
            $("#lot").focus();
            return false;
        }
        if (skuUPC == "") {
            com.FailMsg("#msg", "商品条码不能为空");
            $("#skuUPC").focus();
            return false;
        }
        if (qty == "") {
            com.FailMsg("#msg", "拣货数量不能为空");
            $("#qty").focus();
            return false;
        }
        if (isNaN(qty) || qty <= 0) {
            com.FailMsg("#msg", "请输入正确的拣货数量");
            $("#qty").val("");
            $("#qty").focus();
            return false;
        }
        if ((qty + currentSku.CurrentPickedQty) > currentQty) {
            com.FailMsg("#msg", "拣货数量不能超过出库数量");
            $("#qty").val("");
            $("#qty").focus();
            return false;
        }
        var containerInfo = { StorageCase: container, ContainerQty: qty };
        currentSku.ContainerInfos.push(containerInfo);
        currentSku.CurrentPickedQty = qty + currentSku.CurrentPickedQty;
        picking.DetailList[index] = currentSku;
        if (index + 1 < picking.DetailList.length || currentSku.CurrentPickedQty < currentQty) {
            if ($('#continuousMode').is(':checked')) {
                $("#skuUPC").val("");
                $("#qty").val("");
                $("#skuUPC").focus();
            } else {
                $("#container").val("");
                $("#skuUPC").val("");
                $("#qty").val("");
                $("#container").focus();
            }
            loadSkuInfo(index);
        } else {
            $.router.load("PickingDetailDiff.html?outboundOrder=" + outboundOrder);
        }

        //记录拣货缓存数据
        com.CommonNoLoadingAjax("api/PickDetail/RFSetPickingRedis", {
            OutboundOrder: outboundOrder,
            WarehouseSysId: GetlocalStorage("WarehouseSysId"),
            CurrentUserId: GetlocalStorage("UserId"),
            CurrentDisplayName: GetlocalStorage("DisplayName"),
            PickingDetailList: picking.DetailList
        }, function (data) {

        });
    }

    $("#btnBack").on("click", function () {
        $.router.load("PickingList.html");
    });

    $("#btnViewDiff").on("click", function () {
        $.router.load("PickingDetailDiff.html?outboundOrder=" + outboundOrder);
    });

    $("#btnNext").on("click", function () {
        if (index + 1 < picking.DetailList.length) {
            loadSkuInfo(index + 1);
        } else {
            $.router.load("PickingDetailDiff.html?outboundOrder=" + outboundOrder);
        }
    });
});

//待拣货明细
$(document).on("pageInit", "#page-picking-detaildiff", function (e, pageId, $page) {
    $("#loading").remove();
    $("#msg").html("");
    $("#pickingDetailDiffList").html("");
    var outboundOrder = com.GetQueryString("outboundOrder");
    if (picking.DetailList != null && picking.DetailList.length > 0) {
        var displayIndex = 1;
        var jsonArray = new Array();
        for (var i = 0; i < picking.DetailList.length; i++) {
            if (picking.DetailList[i].Qty != (parseInt(picking.DetailList[i].PickedQty) + parseInt(picking.DetailList[i].CurrentPickedQty))) {
                jsonArray.push('<tr><td style="padding:0.1rem;width:8%">' + displayIndex + '</td>');
                jsonArray.push('<td style="padding:0.1rem 0.2rem;width:48%;word-wrap:break-word;word-break:break-all"><a href="#" style="width:100%;height:100%" data-toggle="popover" data-placement="bottom" data-html="true" data-content="UPC：' + picking.DetailList[i].UPC + "<br/>批次：" + picking.DetailList[i].Lot + '" data-trigger="hover">' + picking.DetailList[i].SkuName + '</a></td>');
                jsonArray.push('<td style="padding:0.1rem 0.2rem;width:24%;word-wrap:break-word;word-break:break-all">' + picking.DetailList[i].Loc + '</td>');
                jsonArray.push('<td style="padding:0.1rem;width:10%">' + picking.DetailList[i].Qty + '</td>');
                jsonArray.push('<td style="padding:0.1rem;width:10%">' + (parseInt(picking.DetailList[i].PickedQty) + parseInt(picking.DetailList[i].CurrentPickedQty)) + '</td></tr>');
                displayIndex++;
            }
        }
        if (jsonArray.length == 0) {
            //com.NoData("#pickingDetailDiffList");
        } else {
            $("#pickingDetailDiffList").append(jsonArray.join(''));

            $('[data-toggle="popover"]').on('click', function (e) {
                jQuery.noConflict()(e.target).popover('toggle')
            });

            jQuery.noConflict()('[data-toggle="popover"]').popover();

            loading = false;
            com.NoScroll();
        }
    } else {
        //com.NoData("#pickingDetailDiffList");
    }

    $("#btnPickFinish").on("click", function () {
        if ($('table tbody tr').length > 0) {
            $.modal({
                title: '拣货完成',
                text: '是否进行差异处理？',
                buttons: [
                  {
                      text: '是',
                      onClick: function () {
                          $.router.load("PickingDetail.html?outboundOrder=" + outboundOrder + "&isDiff=" + "1");
                      }
                  },
                  {
                      text: '否',
                      onClick: function () {
                          pickFinish();
                      }
                  }
                ]
            })
        } else {
            pickFinish();
        }

        function pickFinish() {
            com.CommonNoLoadingAjax("api/PickDetail/RFPickFinish", {
                OutboundOrder: outboundOrder,
                WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                CurrentUserId: GetlocalStorage("UserId"),
                CurrentDisplayName: GetlocalStorage("DisplayName"),
                PickingDetailList: picking.DetailList
            }, function (data) {
                if (data.IsSucess) {
                    $("#msg").html("");
                    if (outboundOrder.trim() != "") {
                        $.router.load("PickingResult.html?outboundOrder=" + outboundOrder);
                    }
                } else {
                    com.FailMsg("#msg", data.Message);
                }
            });
        }
    });

    $("#btnBack").on("click", function () {
        $.router.load("PickingDetail.html?outboundOrder=" + outboundOrder + "&isDiff=" + "1");
    });
});

//拣货结果
$(document).on("pageInit", "#page-picking-result", function (e, pageId, $page) {
    $("#pickedList").html("");
    $("#toPickList").html("");
    $("#pickDiffList").html("");
    var outboundOrder = com.GetQueryString("outboundOrder");
    com.CommonAjax("api/PickDetail/GetPickResult", { OutboundOrder: outboundOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
        if (data != null) {
            setTabTableData("#pickedList", data.PickedList);
            setTabTableData("#toPickList", data.ToPickList);
            setTabTableData("#pickDiffList", data.PickDiffList);

            $('[data-toggle="popover"]').on('click', function (e) {
                jQuery.noConflict()(e.target).popover('toggle')
            });

            jQuery.noConflict()('[data-toggle="popover"]').popover();
        }
        $("#loading").remove();
        $("#loading").remove();
        $("#loading").remove();
    });

    function setTabTableData(tableId, data) {
        if (data != null && data.length > 0) {
            var displayIndex = 1;
            var jsonArray = new Array();
            for (var i = 0; i < data.length; i++) {
                jsonArray.push('<tr><td style="padding:0.1rem;width:8%">' + displayIndex + '</td>');
                jsonArray.push('<td style="padding:0.1rem 0.2rem;width:48%;word-wrap:break-word;word-break:break-all"><a href="#" style="width:100%;height:100%" data-toggle="popover" data-placement="bottom" data-content="UPC：' + data[i].UPC + '" data-trigger="hover">' + data[i].SkuName + '</a></td>');
                //jsonArray.push('<td>' + data[i].UPC + '</td>');
                jsonArray.push('<td style="padding:0.1rem 0.2rem;width:24%;word-wrap:break-word;word-break:break-all">' + data[i].Loc + '</td>');
                jsonArray.push('<td style="padding:0.1rem;width:10%">' +data[i].Qty + '</td>');
                jsonArray.push('<td style="padding:0.1rem;width:10%">' +data[i].PickedQty + '</td></tr>');
                displayIndex++;
            }
            if (jsonArray.length == 0) {
                com.NoData(tableId);
            } else {
                $(tableId).append(jsonArray.join(''));
                loading = false;
                com.NoScroll();
            }
        }
    }

    $("#btnBack").on("click", function () {
        $.router.load("PickingList.html");
    });
});

//复核出库
$(document).on("pageInit", "#page-review-outboundreview", function (e, pageId, $page) {
    sku.OnClose();
    $("#order").focus();
    $("#msg").html("");
    $("#reviewList").html("");

    $("#order").keydown(function () {
        if (event.keyCode == 13) {
            var outboundOrder = $("#order").val();
            com.CommonNoLoadingAjax("api/RFOutbound/CheckOutboundReviewOrder", { OutboundOrder: outboundOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
                if (data.IsSucess) {
                    $("#msg").html("");
                    if (outboundOrder.trim() != "") {
                        selectReviewMode(data.Message);
                    }
                } else {
                    com.FailMsg("#msg", data.Message);
                    $("#order").val("");
                    $("#order").focus();
                }
            });
        }
    });

    function getWaitingReviewList() {
        loading = true;
        com.CommonLoadDataAjax("api/RFOutbound/GetWaitingOutboundReviewListByPaging", {
            CurrentUserId: GetlocalStorage("UserId"),
            WarehouseSysId: GetlocalStorage("WarehouseSysId"),
            iDisplayStart: iDisplayStart,
            iDisplayLength: iDisplayLength
        }, function (data) {
            if (data.TableResuls.aaData != null && data.TableResuls.aaData.length > 0) {
                var jsonArray = new Array();
                for (var i = 0; i < data.TableResuls.aaData.length; i++) {
                    jsonArray.push("<tr onclick='selectReviewMode(\"" + data.TableResuls.aaData[i].OutboundOrder + "\");'>");
                    jsonArray.push('<td>' + (iDisplayStart + (i + 1)) + '</td>');
                    jsonArray.push('<td>' + data.TableResuls.aaData[i].OutboundOrder + '</td>');
                    jsonArray.push('<td>' + (data.TableResuls.aaData[i].ServiceStationName == undefined ? "" : data.TableResuls.aaData[i].ServiceStationName) + '</td>');
                    jsonArray.push('<td>' + data.TableResuls.aaData[i].SkuQty + '</td></tr> ');
                }
                $("#reviewList").append(jsonArray.join(''));
                iTotalDisplayRecords = data.TableResuls.iTotalDisplayRecords;
                loading = false;
                com.NoScroll();
            } else {
                if (data.TableResuls.iTotalDisplayRecords == 0) {
                    com.NoData("#reviewList");
                }
            }
        });
    }

    selectReviewMode = function (outboundOrder) {
        $.router.load("SingleReview.html?outboundOrder=" + outboundOrder);
    }

    //selectReviewMode = function (outboundOrder) {
    //    $.modal({
    //        title: '复核出库',
    //        text: '请选择复核类型',
    //        buttons: [
    //          {
    //              text: '散货复核',
    //              onClick: function () {
    //                  $.router.load("SingleReview.html?outboundOrder=" + outboundOrder);
    //              }
    //          },
    //          {
    //              text: '整件复核',
    //              onClick: function () {
    //                  $.router.load("WholeReview.html?outboundOrder=" + outboundOrder);
    //              }
    //          }
    //        ]
    //    })
    //}

    com.ScrollLoadData(getWaitingReviewList);
    getWaitingReviewList();
});

//散货复核
$(document).on("pageInit", "#page-review-singlereview", function (e, pageId, $page) {
    $("#msg").html("");
    $("#storageCase").val("");
    $("#transferOrder").val("");
    $("#skuUPC").val("");
    $("#qty").val("");
    $("#storageCase").focus();
    var outboundOrder = com.GetQueryString("outboundOrder");

    $("#storageCase").keydown(function () {
        if (event.keyCode == 13) {
            $("#msg").html("");
            if ($("#storageCase").val().trim() != "") {
                getPreBulkPackDetails();
                $("#transferOrder").focus();
            } else {
                com.FailMsg("#msg", "请扫描容器条码");
                // $("#msg").html("请扫描容器条码");
            }
        }
    });

    $("#storageCase").on("blur", function () {
        if ($("#storageCase").val().trim() != "") {
            com.CommonGetAjax("api/RFOutbound/CheckStorageCaseIsAvailable?outboundOrder=" + outboundOrder + "&storageCase=" + $("#storageCase").val().trim() + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
                if (!data.IsSucess) {
                    $("#storageCase").val("");
                    $("#storageCase").focus();
                    com.FailMsg("#msg", data.Message);
                }
            });
        }
    });

    $("#transferOrder").keydown(function () {
        if (event.keyCode == 13) {
            $("#msg").html("");
            if ($("#transferOrder").val().trim() != "") {
                checkTransferOrder();
                $("#skuUPC").focus();
            } else {
                com.FailMsg("#msg", "请扫描交接箱条码");
                //$("#msg").html("请扫描交接箱条码");
            }
        }
    });

    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });

    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            if (!sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#qty", null)) {
                com.FailMsg("#msg", "扫描条码在此单据不存在");
                $("#skuUPC").focus();
                $("#skuUPC").val("");
                return false;
            } else {
                $("#msg").html("");
                $("#qty").focus();
            }
        }
    });

    $("#qty").keydown(function () {
        if (event.keyCode == 13) {
            var storageCase = $("#storageCase").val().trim();
            var transferOrder = $("#transferOrder").val().trim();
            var upc = $("#skuUPC").val().trim();
            var qty = $("#qty").val().trim();
            var skuSysId = sku.GetSkuSysId();

            if (storageCase == "") {
                com.FailMsg("#msg", "容器号不能为空");
                $("#storageCase").focus();
                return false;
            }
            if (transferOrder == "") {
                com.FailMsg("#msg", "交接箱号不能为空");
                $("#transferOrder").focus();
                return false;
            }
            if (upc == "") {
                com.FailMsg("#msg", "商品条码不能为空");
                $("#skuUPC").focus();
                return false;
            }
            if (qty == "") {
                com.FailMsg("#msg", "商品数量不能为空");
                $("#qty").focus();
                return false;
            }
            if (parseFloat(qty) <= 0) {
                com.FailMsg("#msg", "商品数量必须大于0");
                $("#qty").focus();
                return false;
            }
            if (parseFloat(qty) > 9999999) {
                com.FailMsg("#msg", "请输入有效的商品数量");
                $("#qty").focus();
                return false;
            }
            com.CommonNoLoadingAjax("api/RFOutbound/SingleReviewScanning",
                {
                    OutboundOrder: outboundOrder,
                    StorageCase: storageCase,
                    TransferOrder: transferOrder,
                    SkuSysId: skuSysId,
                    UPC: upc,
                    Qty: qty,
                    CurrentUserId: GetlocalStorage("UserId"),
                    CurrentDisplayName: GetlocalStorage("DisplayName"),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                }, function (data) {
                    if (data != null) {
                        if (data.IsSucess) {
                            $("#skuUPC").val("");
                            $("#qty").val("");
                            $("#skuUPC").focus();
                            com.SuccessMsg("#msg", "复核成功");
                        } else {
                            com.FailMsg("#msg", data.Message);
                        }
                    }
                });
        }
    });

    function getPreBulkPackDetails() {
        com.CommonGetAjax("api/RFOutbound/GetStorageCaseSkuList?storageCase=" + $("#storageCase").val().trim() + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
            if (data != null && data.length > 0) {
                sku.SetSkuDetailList(data);
            } else {
                com.FailMsg("#msg", "容器不存在或容器数据已经为空");
                $("#storageCase").focus();
                $("#storageCase").val("");
                $("#transferOrder").val("");
                $("#skuUPC").val("");
                $("#qty").val("");
                $("#loading").remove();
            }
        });
        $("#loading").remove();
    };

    function checkTransferOrder() {
        if ($("#storageCase").val().trim() == "") {
            com.FailMsg("#msg", "容器号不能为空");
            //$("#msg").html("容器号不能为空");
            $("#storageCase").focus();
            return false;
        }
        com.CommonGetAjax("api/RFOutbound/CheckTransferOrderAndPreBulkPack?transferOrder=" + $("#transferOrder").val().trim() + "&storageCase=" + $("#storageCase").val().trim() + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
            if (!data.IsSucess) {
                $("#transferOrder").val("");
                $("#transferOrder").focus();
                com.FailMsg("#msg", data.Message);
            }
        });
    };

    $("#btnBack").on("click", function () {
        $.router.load("OutboundReview.html");
    });

    $("#btnViewDiff").on("click", function () {
        $.router.load("SingleReviewDiff.html?outboundOrder=" + outboundOrder);
    });

    $("#btnSkip").on("click", function () {
        $.modal({
            text: '跳过此步骤将会进入整件复核，是否继续？',
            buttons: [
                {
                    text: '是',
                    onClick: function () {
                        $.router.load("WholeReview.html?outboundOrder=" + outboundOrder);
                    }
                },
                {
                    text: '否',
                    onClick: function () {

                    }
                }
            ]
        })
    });
});

//散件待复核
$(document).on("pageInit", "#page-review-singlereviewdiff", function (e, pageId, $page) {
    $("#msg").html("");
    $("#singleReviewDiffList").html("");
    $("#btnSingleReviewFinish").removeAttr("disabled");
    var outboundOrder = com.GetQueryString("outboundOrder");

    getWaitingSingleReviewDetails();

    function getWaitingSingleReviewDetails() {
        com.CommonNoLoadingAjax("api/RFOutbound/GetWaitingSingleReviewDetails", {
            OutboundOrder: outboundOrder,
            WarehouseSysId: GetlocalStorage("WarehouseSysId"),
            CurrentUserId: GetlocalStorage("UserId"),
            CurrentDisplayName: GetlocalStorage("DisplayName"),
        }, function (data) {
            if (data.length > 0) {
                var isCanFinish = true;
                var displayIndex = 1;
                var jsonArray = new Array();
                for (var i = 0; i < data.length; i++) {
                    jsonArray.push('<tr><td style="padding:0.1rem;width:8%">' + displayIndex + '</td>');
                    jsonArray.push('<td style="display:none">' + data[i].SkuSysId + '</td>');
                    jsonArray.push('<td style="padding:0.1rem 0.2rem;width:68%;word-wrap:break-word;word-break:break-all"><a href="#" style="width:100%;height:100%" data-toggle="popover" data-placement="bottom" data-content="UPC：' +data[i].UPC + '" data-trigger="hover">' +data[i].SkuName + '</a></td>');
                    //jsonArray.push('<td>' + data[i].UPC + '</td>');
                    //jsonArray.push('<td>' + data[i].StorageCase + '</td>');
                    jsonArray.push('<td style="padding:0.1rem;width:12%">' + data[i].PickQty + '</td>');
                    jsonArray.push('<td style="padding:0.1rem;width:12%">' + data[i].ReviewQty + '</td></tr>');
                    displayIndex++;
                    if (parseInt(data[i].PickQty) != parseInt(data[i].ReviewQty)) {
                        isCanFinish = false;
                    }
                }
                $("#singleReviewDiffList").append(jsonArray.join(''));

                $('[data-toggle="popover"]').on('click', function (e) {
                    jQuery.noConflict()(e.target).popover('toggle')
                });

                jQuery.noConflict()('[data-toggle="popover"]').popover();

                if (isCanFinish == false) {
                    $("#btnSingleReviewFinish").attr("disabled", true);
                }
            } else {
                $("#btnSingleReviewFinish").attr("disabled", true);
                com.FailMsg("#msg", "未找到散货待复核明细");
            }
        });
    }

    $("#btnSingleReviewFinish").on("click", function () {
        if ($("#btnSingleReviewFinish").attr("disabled") == undefined) {
            com.CommonNoLoadingAjax("api/RFOutbound/SingleReviewFinish", {
                OutboundOrder: outboundOrder,
                WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                CurrentUserId: GetlocalStorage("UserId"),
                CurrentDisplayName: GetlocalStorage("DisplayName"),
            }, function (data) {
                if (data.IsSucess) {
                    $.router.load("WholeReview.html?outboundOrder=" + outboundOrder);
                } else {
                    com.FailMsg("#msg", data.Message);
                }
            });
        } else {
            return false;
            com.FailMsg("#msg", "复核存在差异，请继续复核");
        }
    });

    $("#btnBack").on("click", function () {
        $.router.load("SingleReview.html?outboundOrder=" + outboundOrder);
    });
});

//整件复核
$(document).on("pageInit", "#page-review-wholereview", function (e, pageId, $page) {
    $("#msg").html("");
    $("#transferOrder").val("");
    $("#skuUPC").val("");
    $("#qty").val("");
    $("#transferOrder").focus();
    var outboundOrder = com.GetQueryString("outboundOrder");

    getOutboundDetails();

    function getOutboundDetails() {
        com.CommonAjax("api/RFOutbound/GetWaitingReviewList", {
            OutboundOrder: outboundOrder,
            WarehouseSysId: GetlocalStorage("WarehouseSysId")
        }, function (data) {
            if (data != null) {
                sku.SetSkuDetailList(data.WaitingReviewList);
            } else {
                com.FailMsg("#msg", "未找到出库单明细");
            }
        });
    }

    function checkTransferOrder() {
        if ($("#transferOrder").val().trim() == "") {
            $("#msg").html("请扫描交接箱条码");
            $("#transferOrder").focus();
            return false;
        }
        com.CommonGetAjax("api/RFOutbound/CheckTransferOrder?transferOrder=" + $("#transferOrder").val().trim() + "&outboundOrder=" + outboundOrder + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
            if (!data.IsSucess) {
                $("#transferOrder").val("");
                $("#transferOrder").focus();
                com.FailMsg("#msg", data.Message);
            }
        });
    };

    $("#transferOrder").keydown(function () {
        if (event.keyCode == 13) {
            $("#msg").html("");
            if ($("#transferOrder").val().trim() != "") {
                checkTransferOrder();
                $("#skuUPC").focus();
            } else {
                $("#msg").html("请扫描交接箱条码");
            }
        }
    });

    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13) {
            $("#qty").focus();
        }
    });

    $("#skuUPC").on("blur", function () {
        if ($("#skuUPC").val().trim() != "") {
            if (!sku.OnPicker($("#skuUPC").val().trim(), "#qty", "#qty", null)) {
                com.FailMsg("#msg", "扫描条码在此单据不存在");
                $("#skuUPC").focus();
                $("#skuUPC").val("");
                return false;
            } else {
                $("#msg").html("");
                $("#qty").focus();
            }
        }
    });

    $("#qty").keydown(function () {
        if (event.keyCode == 13) {
            var transferOrder = $("#transferOrder").val().trim();
            var upc = $("#skuUPC").val().trim();
            var qty = $("#qty").val().trim();
            var skuSysId = sku.GetSkuSysId();

            if (transferOrder == "") {
                com.FailMsg("#msg", "交接箱号不能为空");
                $("#transferOrder").focus();
                return false;
            }
            if (upc == "") {
                com.FailMsg("#msg", "商品条码不能为空");
                $("#skuUPC").focus();
                return false;
            }
            if (qty == "") {
                com.FailMsg("#msg", "商品数量不能为空");
                $("#qty").focus();
                return false;
            }
            if (parseFloat(qty) <= 0) {
                com.FailMsg("#msg", "商品数量必须大于0");
                $("#qty").focus();
                return false;
            }
            if (parseFloat(qty) > 9999999) {
                com.FailMsg("#msg", "请输入有效的商品数量");
                $("#qty").focus();
                return false;
            }
            com.CommonNoLoadingAjax("api/RFOutbound/WholeReviewScanning",
                {
                    OutboundOrder: outboundOrder,
                    TransferOrder: transferOrder,
                    SkuSysId: skuSysId,
                    UPC: upc,
                    Qty: qty,
                    CurrentUserId: GetlocalStorage("UserId"),
                    CurrentDisplayName: GetlocalStorage("DisplayName"),
                    WarehouseSysId: GetlocalStorage("WarehouseSysId")
                }, function (data) {
                    if (data != null) {
                        if (data.IsSucess) {
                            $("#transferOrder").val("");
                            $("#skuUPC").val("");
                            $("#qty").val("");
                            $("#transferOrder").focus();
                            com.SuccessMsg("#msg", "复核成功");
                        } else {
                            com.FailMsg("#msg", data.Message);
                        }
                    }
                });
        }
    });

    $("#btnBack").on("click", function () {
        $.router.load("SingleReview.html?outboundOrder=" + outboundOrder);
    });

    $("#btnViewDiff").on("click", function () {
        $.router.load("WholeReviewDiff.html?outboundOrder=" + outboundOrder);
    });

    $("#btnSkip").on("click", function () {
        $.modal({
            text: '跳过此步骤将会查看整单差异，是否继续？',
            buttons: [
                {
                    text: '是',
                    onClick: function () {
                        $.router.load("OutboundReviewDiff.html?outboundOrder=" + outboundOrder);
                    }
                },
                {
                    text: '否',
                    onClick: function () {

                    }
                }
            ]
        })
    });
});

//整件待复核
$(document).on("pageInit", "#page-review-wholereviewdiff", function (e, pageId, $page) {
    $("#msg").html("");
    $("#wholeReviewDiffList").html("");
    $("#loading").remove();
    $("#btnWholeReviewFinish").removeAttr("disabled");
    var outboundOrder = com.GetQueryString("outboundOrder");

    getWaitingWholeReviewDetails();

    function getWaitingWholeReviewDetails() {
        com.CommonNoLoadingAjax("api/RFOutbound/GetWaitingWholeReviewDetails", {
            OutboundOrder: outboundOrder,
            WarehouseSysId: GetlocalStorage("WarehouseSysId"),
            CurrentUserId: GetlocalStorage("UserId"),
            CurrentDisplayName: GetlocalStorage("DisplayName"),
        }, function (data) {
            if (data.length > 0) {
                var isCanFinish = true;
                var displayIndex = 1;
                var jsonArray = new Array();
                for (var i = 0; i < data.length; i++) {
                    jsonArray.push('<tr><td style="padding:0.1rem;width:8%">' + displayIndex + '</td>');
                    jsonArray.push('<td style="display:none">' + data[i].SkuSysId + '</td>');
                    jsonArray.push('<td style="padding:0.1rem 0.2rem;width:56%;word-wrap:break-word;word-break:break-all"><a href="#" style="width:100%;height:100%" data-toggle="popover" data-placement="bottom" data-content="UPC：' + data[i].UPC + '" data-trigger="hover">' + data[i].SkuName + '</a></td>');
                    //jsonArray.push('<td>' + data[i].UPC + '</td>');
                    jsonArray.push('<td style="padding:0.1rem;width:12%">' + data[i].OutboundQty + '</td>');
                    jsonArray.push('<td style="padding:0.1rem;width:12%">' +data[i].WholeQty + '</td>');
                    jsonArray.push('<td style="padding:0.1rem;width:12%">' +data[i].ReviewQty + '</td></tr>');
                    displayIndex++;
                    if (parseInt(data[i].WholeQty) != parseInt(data[i].ReviewQty)) {
                        isCanFinish = false;
                    }
                }
                $("#wholeReviewDiffList").append(jsonArray.join(''));

                $('[data-toggle="popover"]').on('click', function (e) {
                    jQuery.noConflict()(e.target).popover('toggle')
                });

                jQuery.noConflict()('[data-toggle="popover"]').popover();

                if (isCanFinish == false) {
                    $("#btnWholeReviewFinish").attr("disabled", true);
                }
            } else {
                $("#btnWholeReviewFinish").attr("disabled", true);
                com.FailMsg("#msg", "未找到整件待复核明细");
            }
        });
    }

    $("#btnWholeReviewFinish").on("click", function () {
        if ($("#btnWholeReviewFinish").attr("disabled") == undefined) {
            com.CommonNoLoadingAjax("api/RFOutbound/WholeReviewFinish", {
                OutboundOrder: outboundOrder,
                WarehouseSysId: GetlocalStorage("WarehouseSysId"),
                CurrentUserId: GetlocalStorage("UserId"),
                CurrentDisplayName: GetlocalStorage("DisplayName"),
            }, function (data) {
                if (data.IsSucess) {
                    $.router.load("OutboundReviewDiff.html?outboundOrder=" + outboundOrder);
                } else {
                    com.FailMsg("#msg", data.Message);
                }
            });
        } else {
            return false;
            com.FailMsg("#msg", "复核存在差异，请继续复核");
        }
    });

    $("#btnBack").on("click", function () {
        $.router.load("WholeReview.html?outboundOrder=" + outboundOrder);
    });
});

//整单差异
$(document).on("pageInit", "#page-review-outboundreviewdiff", function (e, pageId, $page) {
    $("#reviewedList").html("");
    $("#toReviewList").html("");
    $("#reviewDiffList").html("");
    var outboundOrder = com.GetQueryString("outboundOrder");
    com.CommonAjax("api/RFOutbound/GetOutboundReviewDiff", { OutboundOrder: outboundOrder, WarehouseSysId: GetlocalStorage("WarehouseSysId") }, function (data) {
        if (data != null) {
            setTabTableData("#reviewedList", data.ReviewedList);
            setTabTableData("#toReviewList", data.ToReviewList);
            setTabTableData("#reviewDiffList", data.ReviewDiffList);
        }
        $("#loading").remove();
        $("#loading").remove();
        $("#loading").remove();
    });

    function setTabTableData(tableId, data) {
        if (data != null && data.length > 0) {
            var displayIndex = 1;
            var jsonArray = new Array();
            for (var i = 0; i < data.length; i++) {
                jsonArray.push('<tr><td style="padding:0.1rem;width:8%">' + displayIndex + '</td>');
                jsonArray.push('<td style="padding:0.1rem 0.2rem;width:56%;word-wrap:break-word;word-break:break-all"><a href="#" style="width:100%;height:100%" data-toggle="popover" data-placement="bottom" data-content="UPC：' + data[i].UPC + '" data-trigger="hover">' + data[i].SkuName + '</a></td>');
                //jsonArray.push('<td>' + data[i].UPC + '</td>');
                jsonArray.push('<td style="padding:0.1rem;width:12%">' + data[i].OutboundQty + '</td>');
                jsonArray.push('<td style="padding:0.1rem;width:12%">' + data[i].PickQty + '</td>');
                jsonArray.push('<td style="padding:0.1rem;width:12%">' + data[i].ReviewQty + '</td></tr>');
                displayIndex++;
            }
            if (jsonArray.length == 0) {
                com.NoData(tableId);
            } else {
                $(tableId).append(jsonArray.join(''));

                $('[data-toggle="popover"]').on('click', function (e) {
                    jQuery.noConflict()(e.target).popover('toggle')
                });

                jQuery.noConflict()('[data-toggle="popover"]').popover();

                loading = false;
                com.NoScroll();
            }
        }
    }

    $("#btnBack").on("click", function () {
        $.router.load("OutboundReview.html");
    });
});

//商品包装管理
$(document).on("pageInit", "#page-basics-packmangerindex", function (e, pageId, $page) {

    $("#skuName").val('');
    $("#skuUPC").val("");
    $("#skuUPC").focus();
    $("#UPC03").val("");
    $("#FieldValue03").val("");
    $("#msg").html("");
    $("#packCode").val("");
    $("#packSysId").val("");
    $("#upc03repeat").val("");

    getAllUOM();

    $("#skuUPC").keydown(function () {
        if (event.keyCode == 13 && $("#skuUPC").val().trim() != "") {

            com.CommonNoLoadingGetAjax("api/Base/GetSkuListByUPC?upc=" + $("#skuUPC").val().trim() + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
                if (data != null && data.length > 0) {
                    $("#msg").html("");
                    sku.SetSkuDetailList(data);
                    if (!sku.OnPicker($("#skuUPC").val().trim(), null, null, null, function () {
                        for (var i = 0; i < sku.SkuDetailList.length; i++) {
                             if (sku.SkuDetailList[i].SkuSysId == sku.GetSkuSysId()) {
                                $("#skuName").val(sku.SkuDetailList[i].SkuName);
                    }
                    }
                        getSkuPackInfo();
                    })) {
                        com.FailMsg("#msg", "商品不存在");
                        $("#skuUPC").focus();
                        $("#skuUPC").val("");
                        $("#shuName").html("");
                        return false;
                    }

                } else {
                    com.FailMsg("#msg", "商品不存在");
                    $("#skuUPC").val("");
                    $("#skuUPC").focus();
                    return false;
                }
            });

        }
    });

    ///获取sku包装信息
    function getSkuPackInfo() {
        com.CommonNoLoadingGetAjax("api/Base/GetPackBySkuSysId?skuSysId=" + sku.GetSkuSysId() + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
            if (data != null && data.SysId != null && data.SysId != undefined && data.SysId != '') {
                //根据商品获取到包赚信息
                $("#FieldUom01").find("option[value = '" + data.FieldUom01 + "']").attr("selected", "selected");
                $("#FieldUom03").find("option[value = '" + data.FieldUom03 + "']").attr("selected", "selected");

                if (data.UPC03 != '' && data.UPC03 != undefined) {
                    $("#UPC03").val(data.UPC03);
                }
                if (data.PackCode != '' && data.PackCode != undefined) {
                    $("#packCode").val(data.PackCode);
                }
                if (data.FieldValue03 != '' && data.FieldValue03 != undefined) {
                    $("#FieldValue03").val(data.FieldValue03);
                }
                $("#packSysId").val(data.SysId);
                $("#UPC03").focus();
            } else {

                //设置基本单位默认选择为缺省
                $("#FieldUom01 option").each(function () {
                    if ($(this).text() == "缺省") {
                        $(this).attr("selected", "selected");
                    }
                });

                //设置外包装单位默认选择为缺省
                $("#FieldUom03 option").each(function () {
                    if ($(this).text() == "缺省") {
                        $(this).attr("selected", "selected");
                    }
                });

                $("#packSysId").val('');
                $("#UPC03").val('');
                $("#UPC03").focus();
                $("#packCode").val($("#skuName").val() + '包装');
                $("#FieldValue03").val('');
            }
        });
    }

    ///获取所有基本包装单位
    function getAllUOM() {
        com.CommonNoLoadingAjax("api/BaseData/Package/GetUOMList", { iDisplayStart: 0, iDisplayLength: 999 }, function (data) {
            if (data.TableResuls != null) {
                var array = new Array;
                for (var i = 0; i < data.TableResuls.aaData.length; i++) {
                    array.push('<option value="' + data.TableResuls.aaData[i].SysId + '">' + data.TableResuls.aaData[i].UOMCode + '</option>')
                }

                //基本包装单位
                $("#FieldUom01").html('');
                $("#FieldUom01").html(array.join(''));

                //设置基本单位默认选择为缺省
                $("#FieldUom01 option").each(function () {
                    if ($(this).text() == "缺省") {
                        $(this).attr("selected", "selected");
                    }
                });

                //外包装单位
                $("#FieldUom03").html('');
                $("#FieldUom03").html(array.join(''));

                //设置外包装单位默认选择为缺省
                $("#FieldUom03 option").each(function () {
                    if ($(this).text() == "缺省") {
                        $(this).attr("selected", "selected");
                    }
                });

            }
        });
    }

    //检测外包装UPC是否重复
    $("#UPC03").keydown(function () {
        if (event.keyCode == 13 && $("#UPC03").val().trim() != "") {
            $("#packCode").focus();
            /*
            //com.CommonNoLoadingGetAjax("api/Base/GetPackListByUPC?upc=" + $("#UPC03").val() + "&warehouseSysId=" + GetlocalStorage("WarehouseSysId"), function (data) {
            //    if (data != null && data.length > 0) {
            //        if (data.length == 1) {
            //            //根据外包装UPC获取到包装信息中不存在重复
            //            $("#FieldUom01").find("option[value = '" + data[0].FieldUom01 + "']").attr("selected", "selected");
            //            $("#FieldUom03").find("option[value = '" + data[0].FieldUom03 + "']").attr("selected", "selected");
            //            //$("#FieldValue03").val(data[0].FieldValue03);
            //            //$("#UPC03").val(data[0].UPC03);
            //            //$("#packCode").val(data[0].PackCode);

            //            if (data[0].PackCode != '' && data[0].PackCode != undefined) {
            //                $("#packCode").val(data[0].PackCode);
            //            } else {
            //                $("#packCode").val($("#skuName").val() + '包装');
            //            }
            
            //            if (data[0].FieldValue03 != '' && data[0].FieldValue03 != undefined) {
            //                $("#FieldValue03").val(data[0].FieldValue03);
            //            }

            //            $("#packSysId").val(data[0].SysId);

            //        } else {  //根据外包装UPC获取到包装信息存在UPC03重复

            //            var listdata = data;
            //            var upcValueArray = new Array();
            //            var upcTextArray = new Array();
            //            for (var i = 0; i < listdata.length; i++) {
            //                if (listdata[i].SysId != '' && listdata[i].UPC03 != '') {
            //                    upcValueArray.push(listdata[i].SysId);
            //                    upcTextArray.push("包装:" + listdata[i].PackCode + ";数量:" + listdata[i].FieldValue03);
            //                }
            //            }
            //            //设置第一项为选择项
            //            $("#upc03repeat").val(upcValueArray[0]);

            //            if ($("#packSysId").val() != '') {
            //                //如果sku有包装，则吧sku包装设置为选择项
            //                $("#upc03repeat").val($("#packSysId").val());
            //            }

            //            $("#upc03repeat").picker({
            //                toolbarTemplate: '<header class="bar bar-nav">\
            //                  <button class="button button-link pull-left cancel-picker pad-l-r-10 ">取消</button>\
            //                  <button class="button button-link pull-right close-picker pad-l-r-10 ">确定</button>\
            //                  <h1 class="title">请选择</h1>\
            //                  </header>',
            //                cols: [
            //                  {
            //                      textAlign: 'center',
            //                      values: upcValueArray,
            //                      displayValues: upcTextArray
            //                  }
            //                ],
            //                onClose: function () {
            //                    if (flag) {  //当包装UPC重复时，选择其中一个
            //                        for (var i = 0; i < listdata.length; i++) {
            //                            if (listdata[i].SysId == $("#upc03repeat").val()) {
            //                                $("#FieldUom01").find("option[value = '" + listdata[i].FieldUom01 + "']").attr("selected", "selected");
            //                                $("#FieldUom03").find("option[value = '" + listdata[i].FieldUom03 + "']").attr("selected", "selected");

            //                                //$("#FieldValue03").val(listdata[i].FieldValue03);
            //                                //$("#UPC03").val(listdata[i].UPC03);
            //                                //$("#packCode").val(listdata[i].PackCode);

            //                                if (listdata[i].PackCode != '' && listdata[i].PackCode != undefined) {
            //                                    $("#packCode").val(listdata[i].PackCode);
            //                                } else {
            //                                    $("#packCode").val($("#skuName").val() + '包装');
            //                                }
            //                                if (listdata[i].FieldValue03 != '' && listdata[i].FieldValue03 != undefined) {
            //                                    $("#FieldValue03").val(listdata[i].FieldValue03);
            //                                }

            //                                $("#packSysId").val(listdata[i].SysId);
            //                                $("#packCode").focus();
            //                            }
            //                        }
            //                    }
            //                    else {      //当包装UPC重复时，不选择
            //                        $("#upc03repeat").val('');
            //                        $("#packSysId").val('')
            //                        $("#packCode").val($("#skuName").val() + '包装');
            //                        $("#FieldValue03").val('');
            //                        $("#FieldValue03").focus();
            //                        flag = true;
            //                    }
            //                }
            //            });
            //            $("#upc03repeat").picker("open");

            //            $(".cancel-picker").on('click', function () {
            //                flag = false;
            //                $("#upc03repeat").picker("close");
            //            });
            //        }
            //    }
            //    else { //根据包装UPC没有获取到UPC
            //        $("#packSysId").val('')
            //        $("#packCode").val($("#skuName").val() + '包装');
            //        $("#FieldValue03").val('');
            //        $("#FieldValue03").focus();
            //    }
            //});
            */
        }
    });

    $("#packCode").keydown(function () {
        if (event.keyCode == 13) {
            $("#FieldValue03").focus();
        }
    });

    var submite = false;
    //保存
    $("#btnStartReview").on('click', function () {
        $("#msg").html('');
        var skuUPC = $("#skuUPC").val().trim();
        if (skuUPC == '') {
            com.FailMsg("#msg", "商品条码不能为空");
            return false;
        }

        var upc03 = $("#UPC03").val().trim();
        if (upc03 == '') {
            com.FailMsg("#msg", "外包装条码不能为空");
            return false;
        }
        if (upc03.length > 31) {
            com.FailMsg("#msg", "外包装条码过长");
            return false;
        }

        var packCode = $("#packCode").val().trim();
        if (packCode == '') {
            com.FailMsg("#msg", "外包装名称不能为空");
            return false;
        }

        var fieldValue03 = $("#FieldValue03").val().trim();
        if (fieldValue03 == '') {
            com.FailMsg("#msg", "外包装数量不能为空");
            return false;
        }
        if (parseInt(fieldValue03) <= 0) {
            com.FailMsg("#msg", "外包装数量必须大于0");
            return false;
        }

        if (parseInt(fieldValue03) > 9999999) {
            com.FailMsg("#msg", "请输入有效的外包装数量");
            return false;
        }

        var datas = {
            SkuSysId: sku.GetSkuSysId(),
            FieldValue01: $("#FieldValue01").val().trim(),
            FieldUom01: $("#FieldUom01").val().trim(),
            PackSysId: $("#packSysId").val(),
            PackCode: packCode,
            FieldValue03: fieldValue03,
            FieldUom03: $("#FieldUom03").val().trim(),
            UPC03: upc03,
            WareHouseSysId: GetlocalStorage("WarehouseSysId")
        };
        if (submite) {
            return false;
        }
        submite = true;
        com.CommonAjax("api/Base/UpdateSkuPack", datas, function (data) {
            submite = false;
            if (data.IsSucess) {
                $("#skuName").val('');
                $("#skuUPC").val("");
                $("#skuUPC").focus();
                $("#UPC03").val("");
                $("#FieldValue03").val("");
                $("#packCode").val("");
                $("#packSysId").val("");
                $("#upc03repeat").val("");

                //设置基本单位默认选择为缺省
                $("#FieldUom01 option").each(function () {
                    if ($(this).text() == "缺省") {
                        $(this).attr("selected", "selected");
                    }
                });

                //设置外包装单位默认选择为缺省
                $("#FieldUom03 option").each(function () {
                    if ($(this).text() == "缺省") {
                        $(this).attr("selected", "selected");
                    }
                });

                $("#msg").html("");

            } else {
                com.FailMsg("#msg", data.Message);
                return false;
            }
        });
    });
});
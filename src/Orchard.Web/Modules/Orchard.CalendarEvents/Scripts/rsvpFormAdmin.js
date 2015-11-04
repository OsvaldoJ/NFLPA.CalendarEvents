var rsvpInvitesApp = angular.module('rsvpInvitesApp', []);

if (typeof String.prototype.startsWith != 'function') {
    String.prototype.startsWith = function (prefix) {
        return this.slice(0, prefix.length) == prefix;
    };
}

if (typeof String.prototype.endsWith != 'function') {
    String.prototype.endsWith = function (suffix) {
        return this.slice(-suffix.length) == suffix;
    };
}

rsvpInvitesApp.controller('RsvpInvitesCtrl', function ($scope, $filter, documentService, filteredListService) {

    var allItemsInput = null;
    $scope.allItemsString = null;
    $scope.currentPage = 1;
    $scope.resultsPerPage = 10;
    $scope.overwriteList = false;
    $scope.allItems = [];
    $scope.filteredList = [];
    $scope.searchResults = [];
    $scope.totalItems = $scope.allItems.length;
    $scope.numberofPages = Math.floor($scope.searchResults.length / $scope.resultsPerPage);

    $scope.init = function (fieldId) {

        allItemsInput = $('#' + fieldId);
        $scope.allItemsString = allItemsInput.val();
        if($scope.allItemsString)
            $scope.allItems = JSON.parse($scope.allItemsString);

        var $drop = $('#dropZone');
        var drop = $drop[0];
        drop.addEventListener('drop', $scope.handleDrop, false);
        drop.addEventListener('dragenter', $scope.handleDragover, false);
        drop.addEventListener('dragover', $scope.handleDragover, false);

        $drop.on('dragenter', function (e) {
            e.preventDefault();
            $(this).addClass('hover');
        });
        $drop.on('dragleave', function (e) {
            e.preventDefault();
            $(this).removeClass('hover');
        });

        var fileInput = $('#templateUploadInput')[0];
        fileInput.addEventListener('change', function (e1) {
            var f = fileInput.files[0];
            documentService.processFile(f, $scope);
        });
        $scope.resetFilters();
    }

    $scope.updateList = function (data, scope) {
        if (scope == null) {
            scope = $scope;
        }
        $('#dropZone').removeClass('hover');
        if (scope.overwriteList)
            scope.allItems = data;
        else {
            var filtered = data.filter(filteredListService.isNewInvite);
            filtered.forEach(function (obj) {
                scope.allItems.push(obj);
            });
        }

        scope.allItemsString = JSON.stringify(scope.allItems);
        allItemsInput.val(scope.allItemsString);
        scope.resetFilters();
        $('#templateUploadInput')[0].value = '';
    }

    $scope.resetFilters = function () {
        $scope.filteredList = $scope.searchResults = $scope.allItems;
        $scope.currentPage = 1;
        $scope.totalItems = $scope.allItems.length;
        $scope.numberofPages = Math.floor($scope.searchResults.length / $scope.resultsPerPage);
        console.log($scope.filteredList);
    }

    $scope.paginate = function () {
        var index = ($scope.currentPage - 1) * $scope.resultsPerPage;
        $scope.filteredList = $scope.searchResults.splice(index, $scope.resultsPerPage);
    }

    $scope.range = function (start, end) {
        var ret = [];
        if (!end) {
            end = start;
            start = 0;
        }
        for (var i = start; i < end; i++) {
            ret.push(i + 1);
        }
        return ret;
    };

    $scope.prevPage = function () {
        if ($scope.currentPage > 1) {
            $scope.currentPage--;
            $scope.paginate();
        }
    };

    $scope.nextPage = function () {
        if ($scope.currentPage < $scope.numberofPages) {
            $scope.currentPage++;
            $scope.paginate();
        }
    };

    $scope.setPage = function () {
        $scope.currentPage = this.n;
        $scope.paginate();
    };


    $scope.clearInvites = function () {
        if (confirm("Are you sure you want to remove everyone from the invite list?")) {
            $scope.allItems = [];
            $scope.resetFilters();
        }
    }

    $scope.downloadInvites = function () {
        documentService.downloadTemplate($scope.allItems, "RsvpInviteList");
    }

    $scope.sendInvites = function () {
        console.log("send invites clicked");
    }
    $scope.sendInvite = function (idx) {
        var inviteToEmail = $scope.filteredList[idx];
        console.log(inviteToEmail);
    }
    $scope.remove = function (idx) {
        var inviteToRemove = $scope.filteredList[idx];
        console.log(inviteToRemove);
    }

    $scope.downloadTemplate = function () {
        documentService.downloadTemplate([]);
    }

    $scope.handleDrop = function(e) {
        e.stopPropagation();
        e.preventDefault();
        var files = e.dataTransfer.files;
        var i, f;
        f = files[0];
        documentService.processFile(f, $scope);
    }
    $scope.handleDragover = function (e) {
        e.stopPropagation();
        e.preventDefault();
        e.dataTransfer.dropEffect = 'copy';
    }

    //services

});

rsvpInvitesApp.service('documentService',["$rootScope", function($rootScope) {

    this.processCsv = function(csv) {
        var lines = csv.split(/\r?\n/);
        var result = [];
        var headers = lines[0].split(",");
        for (var i = 1; i < lines.length; i++) {
            var obj = {};
            var currentline = lines[i].split(",");
            if (currentline.length == headers.length) {
                for (var j = 0; j < headers.length; j++) {
                    obj[headers[j]] = currentline[j];
                }
                if (obj != {})
                    result.push(obj);
            }
        }
        return result;
    }
    this.processFile = function (f, $scope) {
        var reader = new FileReader();
        if (f) {
            var name = f.name;
            var type = f.type;

            reader.onload = function (e) {
                var data = e.target.result;
                /* if binary string, read with type 'binary' */
                var result = {};
                if (type.startsWith('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')) {
                    var workbook = XLSX.read(data, { type: 'binary' });
                    workbook.SheetNames.forEach(function (sheetName) {
                        var roa = XLSX.utils.sheet_to_row_object_array(workbook.Sheets[sheetName]);
                        if (roa.length > 0) {
                            result["inviteList"] = roa;
                        }
                    });
                }
                else if (type == 'application/vnd.ms-excel' && name.endsWith('.csv')) {
                    //assume csv
                    var jsonData = processCsv(data);
                    if (jsonData.length > 0) {
                        result["inviteList"] = jsonData;
                    }
                }
                //console.log(result.inviteList);
                console.log($scope);
                return $scope.updateList(result.inviteList);
            };
            reader.readAsBinaryString(f);
        }
    };
    this.downloadTemplate = function (data, filename) {
        this.exporter.save(data, filename);
    }
    this.Workbook = function () {
        if (!(this instanceof Workbook))
            return new Workbook();
        this.SheetNames = [];
        this.Sheets = {};
    }
    this.exporter = {
        datenum: function (v, date1904) {
            if (date1904) v += 1462;
            var epoch = Date.parse(v);
            return (epoch - new Date(Date.UTC(1899, 11, 30))) / (24 * 60 * 60 * 1000);
        },
        buildSheet: function (data, opts) {
            if (data.length == 0) {
                data = [
                    {
                        FirstName: "Example",
                        LastName: "Example",
                        EmailAddress: "example@nflpa.com",
                        VIP: "false",
                        MaxGuests: "0",
                        InviteSentThroughOrchard: "true",
                        Responded: "false"
                    }
                ];
            }

            var ws = {};
            var range = { s: { c: 10000000, r: 10000000 }, e: { c: 0, r: 0 } };
            //set up headers

            var arrayOfData = [];
            var headerRow = [];
            for (property in data[0]) {
                //row = 0;
                if (data[0].hasOwnProperty(property)) {
                    headerRow.push(property);
                }
            }
            arrayOfData.push(headerRow);
            for (var row = 0; row != data.length; row++) {
                var contentRow = [];
                for (key in data[row]) {
                    if (data[row].hasOwnProperty(key)) {
                        var val = data[row][key];
                        contentRow.push(val);
                    }
                }
                arrayOfData.push(contentRow);
            }

            //now iterate through array of data
            for (var R = 0; R != arrayOfData.length; ++R) {
                for (var C = 0; C != arrayOfData[R].length; ++C) {
                    if (range.s.r > R) range.s.r = R;
                    if (range.s.c > C) range.s.c = C;
                    if (range.e.r < R) range.e.r = R;
                    if (range.e.c < C) range.e.c = C;
                    var cell = { v: arrayOfData[R][C] };
                    if (cell.v == null) cell.v = '';
                    var cell_ref = XLSX.utils.encode_cell({ c: C, r: R });

                    if (parseInt(cell.v).toString() == cell.v) cell.t = 'n';
                    else if (cell.v.toLowerCase == 'true' || cell.v.toLowerCase == 'false') cell.t = 'b';
                    else if (cell.v instanceof Date) {
                        cell.t = 'n'; cell.z = XLSX.SSF._table[14];
                        cell.v = datenum(cell.v);
                    }
                    else cell.t = 's';

                    ws[cell_ref] = cell;
                }
            }
            if (range.s.c < 10000000) ws['!ref'] = XLSX.utils.encode_range(range);

            return ws;
        },
        /* original data */
        s2ab: function (s) {
            var buf = new ArrayBuffer(s.length);
            var view = new Uint8Array(buf);
            for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
            return buf;
        },
        save: function (data, filename) {
            if (filename == null)
                filename = "RsvpInvitesTemplate";
            var wb = new Workbook();
            wb.SheetNames = ["RsvpInvites"];
            wb.Sheets["RsvpInvites"] = this.buildSheet(data);
            var wbout = XLSX.write(wb, { bookType: 'xlsx', bookSST: true, type: 'binary' });
            saveAs(new Blob([this.s2ab(wbout)], { type: "application/octet-stream" }), filename + ".xlsx");
        }
    }
}]);

function Workbook() {
    if (!(this instanceof Workbook))
        return new Workbook();
    this.SheetNames = [];
    this.Sheets = {};
}

rsvpInvitesApp.service('filteredListService', ["$rootScope", function ($rootScope) {
    
    this.isNewInvite = function (newObject) {
        return !$rootScope.$$childHead.allItems.some(function (currentObject) {
            return newObject.EmailAddress == currentObject.EmailAddress;
        });
    }

    this.searched = function (valLists, toSearch) {
        return _.filter(valLists,
        function (i) {
            /* Search Text in all 3 fields */
            return searchUtil(i, toSearch);
        });
    };

    this.paged = function (valLists, pageSize) {
        var retVal = [];
        for (var i = 0; i < valLists.length; i++) {
            if (i % pageSize === 0) {
                retVal[Math.floor(i / pageSize)] = [valLists[i]];
            } else {
                retVal[Math.floor(i / pageSize)].push(valLists[i]);
            }
        }
        return retVal;
    };

    this.searchUtil = function (item, toSearch) {
        /* Search Text in all 3 fields */
        return (item.name.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
            item.Email.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
            item.EmpId == toSearch) ? true : false;
    }

}]);

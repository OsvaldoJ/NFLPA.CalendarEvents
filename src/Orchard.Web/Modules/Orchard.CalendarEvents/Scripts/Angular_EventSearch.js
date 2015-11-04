var app = angular.module('EventSearchApp',
    ["kendo.directives","ui.bootstrap"]);

app.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter);
                });

                event.preventDefault();
            }
        });
    };
});

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


app.controller('EventSearchCtrl', function ($scope, $http) {

    $scope.enableSearch = false;
    $scope.loaded = false;
    $scope.eventTypes = null;
    $scope.eventTypeFilter = null;
    $scope.title = null;
    $scope.currentPage = 1;
    $scope.numberofPages = 1;
    $scope.totalItems = 0;
    $scope.resultsPerPage = 5;
    $scope.ranSearch = false;
    $scope.calendarId = "";
    $scope.rootUrl = "";
    $scope.apiUrl = function() {
        return $scope.calendarId.length > 0
            ? $scope.rootUrl + "/api/Events/SearchCalendar/" + $scope.calendarId
            : $scope.rootUrl + "/api/Events/Search";
    }

    $scope.EventSelectorInit = function (id, rootUrl) {
        if (rootUrl != "/")
            $scope.rootUrl = rootUrl;
        $http.get($scope.rootUrl + "/api/Events/GetEventCategories").success(function (data) {
            if (data == "null")
                data = [];

            $scope.eventTypes = data;
            $scope.eventTypeFilter = $scope.eventTypes[0].Identifier;

        }).error(function () {
            $scope.eventTypes = [];
        });

        if (id != null && id.length > 1) {
            $http.get($scope.rootUrl + "/api/Events/GetEventByIdentifier/" + id).success(function (data) {
                $scope.selectedEvent = data;
                $scope.changingSelectedEvent = data == null;
                $scope.enableSearch = true;
            }).error(function () {
                $scope.selectedEvent = null;
            });
        }

        $scope.loaded = true;
        
    }

    $scope.init = function (autoSearch, calendarId, rootUrl) {
        if (rootUrl != "/")
            $scope.rootUrl = rootUrl;
        $http.get($scope.rootUrl + "/api/Events/GetEventCategories").success(function (data) {
            if (data == "null")
                data = [];
            var array = [{ Identifier: '', CategoryName: "All Events" }];
            data.forEach(function(element, index, arr) {
                array.push(element);
            });
            $scope.eventTypes = array;
            $scope.eventTypeFilter = $scope.eventTypes[0].Identifier;

        }).error(function () {
            $scope.eventTypes = [];
        });
        $scope.calendarId = calendarId;

        $scope.loaded = true;
        $scope.autoSearch = autoSearch;
        if ($scope.autoSearch != null) {
            $scope.newSearchEvents();
        }

    };

    $scope.stringValue = function () {
        if ($scope.selectedEvent && $scope.enableSearch)
            return $scope.selectedEvent.Identifier;
        return '';
    }

    $scope.categoriesForEvent = function (categoryNamesCsv) {
        if (categoryNamesCsv)
            return categoryNamesCsv.split(',');
        return [];
    }

    $scope.changingSelectedEvent = !($scope.selectedEvent != null);

    $scope.changeSelectedEvent = function () {
        $scope.changingSelectedEvent = true;
    }

    $scope.showSearch = function () {
        return $scope.changingSelectedEvent;
    }

    $scope.addItem = function (idx) {
        var itemToAdd = $scope.searchResults[idx];
        $scope.selectedEvent = (itemToAdd);
        $scope.changingSelectedEvent = false;
        $scope.resetSearch();
    }

    $scope.cancelSearch = function () {
        if ($scope.selectedEvent)
            $scope.changingSelectedEvent = false;
        $scope.ranSearch = false;
        $scope.resetSearch();
    }
    $scope.newSearchEvents = function () {
        $scope.currentPage = 1;
        $scope.numberofPages = 1;
        $scope.totalItems = 0;
        $scope.searchEvents();
    }
    $scope.searchEvents = function () {
        $scope.ranSearch = true;
        $http({
            url: $scope.apiUrl(),
            method: "GET",
            params: {
                title: $scope.title,
                startDate: $scope.startDateFilter,
                endDate: $scope.endDateFilter,
                eventCategoryIdsCsv: $scope.eventTypeFilter,
                currentPage: $scope.currentPage,
                resultsPerPage: $scope.resultsPerPage
            }
        }).success(function (data) {
            if (data == "null")
                data = [];

            $scope.searchResults = data.SearchResults;
            $scope.currentPage = data.CurrentPage;
            $scope.numberofPages = data.NumberofPages;
            $scope.totalItems = data.TotalItems;
            $scope.resultsPerPage = data.ResultsPerPage;

        }).error(function () {
            $scope.searchResults = [];
        });
    }
    $scope.isDate = function (x) {
        return x instanceof Date;
    };
    $scope.resetSearch = function () {
        $scope.title = "";
        $scope.startDateFilter = "";
        $scope.endDateFilter = "";
        $scope.searchResults = [];
        $scope.currentPage = 1;
        $scope.numberofPages = 1;
        $scope.totalItems = 0;
        $scope.ranSearch = false;
        if ($scope.autoSearch != null) {
            $scope.newSearchEvents();
        }
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
            $scope.searchEvents();
        }
    };

    $scope.nextPage = function () {
        if ($scope.currentPage < $scope.numberofPages) {
            $scope.currentPage++;
            $scope.searchEvents();
        }
    };

    $scope.setPage = function () {
        $scope.searchEvents();
    };

    $scope.convertDate = function (date, format) {
        if (format == undefined)
            format = 'dddd';
        var mday = moment(date).utc();
        return mday.format(format);
    };

    $scope.showTwoDates = function(item) {
        var format = "MM/DD/YYYY";
        var start = $scope.convertDate(item.Start, format);
        var end = $scope.convertDate(item.End, format);
        var sameDay = start == end;
        return !sameDay;
    }
});
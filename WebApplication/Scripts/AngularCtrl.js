var app = angular.module("MyApp",[])
app.controller("ExcelCtrl", ['$scope', 'Excelservice', function ($scope, Excelservice) {
    $scope.SelectedFileForUpload = null; //initially make it null
    $scope.BindData = null;
    $scope.showLoader = false;
    $scope.IsVisible = false;
    $scope.UploadFiles = function (files) {
        $scope.$apply(function () {
            $scope.Message = '';
            $scope.SelectedFileForUpload = files[0];
        })
    }
    $scope.ParseExcel = function () {
        var formData = new FormData();
        var file = $scope.SelectedFileForUpload;
        formData.append('file', file);
        $scope.showLoader = true;   //show spinner
        var response = Excelservice.SendExcelData(formData);   //calling service
        response.then(function (d) {
            var res = d.data;
            $scope.BindData = res;
            $scope.IsVisible = true; //showing the table after databinding
            $scope.showLoader = false; //after success hide spinner
        }, function (err) {
            console.log(err.data);
            console.log("error in parse excel");
        });
    }

    $scope.InsertData = function () {
        var response = Excelservice.InsertToDB();
        response.then(function (d) {
            var res = d.data;

            if (res.toString().length > 0) {
                $scope.Message = res + "  Records Inserted";
                $scope.IsVisible = false;   //used to disable the insert button and table after submitting data
                angular.forEach(
                angular.element("input[type='file']"),
                function (inputElem) {
                    angular.element(inputElem).val(null); //used to clear the file upload after submitting data
                });
                $scope.SelectedFileForUpload = null;   //used to disable the preview button after inserting data
            }

        }, function (err) {
            console.log(err.data);
            console.log("error in insertdata");
        });
    }

}])
app.service("Excelservice", function ($http) {
    this.SendExcelData = function (data) {
        var request = $http({
            method: "post",
            withCredentials: true,
            url: '/Group/ReadExcel',
            data: data,
            headers: {
                'Content-Type': undefined
            },
            transformRequest: angular.identity
        });
        return request;
    }
    this.InsertToDB = function () {
        var request = $http({
            method: "get",
            url: '/Group/InsertExcelData',
            data: {},
            datatype: 'json'
        });
        return request;
    }
})


//used to check the extension of file while uploading
function checkfile(sender) {
    var validExts = new Array(".xlsx", ".xls");
    var fileExt = sender.value;
    fileExt = fileExt.substring(fileExt.lastIndexOf('.'));
    if (validExts.indexOf(fileExt) < 0) {
        alert("Invalid file selected, valid files are of " +
                 validExts.toString() + " types.");
        return false;
    }
    else return true;
}

var app = angular.module('jkApp', ['ngMessages']);

app.controller('crmContactsController', function ($scope, $http) {
    // Variables
    $scope.contactTypes = {};
    $scope.newContact = {};

    // Functions
    $scope.GetContacts = function () {
        $http.get("CRM/CRMContacts/GetContacts").then(
            function successCallback() {


                $("ul#contact-type-ul li:first-child").addClass("active");
            },
            function errorCallback() {

            });
    };

    GetContacts();
});
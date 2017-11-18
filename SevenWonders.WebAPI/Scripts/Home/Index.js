

function CurrentUserModel()
{
    var self = this;

    self.email = ko.observable("");
    self.role = ko.observable(0);
    

    self.updateCurrentUser = function() {

        $.ajax({
            type: "GET",
            url: "../api/Account/GetCurrentUser",
            success: function (data) {
                if (data != null) {
                    self.email(data.Email);
                    self.role(data.RoleId);
                }
                else {
                    self.email("");
                    self.role(0);
                }
            },
            error: function (err) {
                console.log(err);
            }
        });

        self.visibleForCustomers = ko.computed(function () {
            if (self.role() === 1) return true;
            return false;
        });

        self.visibleForGuests = ko.computed(function () {
            if (self.role() === 0) return true;
            return false;
        });

        self.visibleForManagers = ko.computed(function () {
            if (self.role() === 3) return true;
            return false;
        });

        self.visibleForAdmins = ko.computed(function () {
            if (self.role() === 2) return true;
            return false;
        });
    }

    self.updateCurrentUser();
}

ko.unapplyBindings = function ($node, remove) {
    // unbind events
    $($node).find("*").each(function () {
        $(this).unbind();
    });

    // Remove KO subscriptions and references
    if (remove) {
        ko.removeNode($node[0]);
    } else {
        ko.cleanNode($node[0]);
    }
};

function FakeModel(){}

var currentUserModelObject = new CurrentUserModel();
//ko.applyBindings(currentUserModelObject);
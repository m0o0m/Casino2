(function (sportjuicecontrol, $) {
    var settings = {
        itemDOM: '<tr>' +
                    '<td><select class="sport"></select></td>' +
                    '<td><input type="text" value="0" onkeypress="javascript:return inputNumber(this, event, true);" style="width: 90px;" /></td>' +
                    '<td><select class="bettype"></select></td>' +
                    '<td><input type="checkbox" class="gm" data-jid="" data-cate="" /></td>' +
                    '<td><input type="checkbox" class="1h" data-jid="" data-cate="" /></td>' +
                    '<td><input type="checkbox" class="2h" data-jid="" data-cate="" /></td>' +
                    '<td><a href="#" class="btn btn-red btn-sm mlm mrm" onclick="sportjuicecontrol.deleteRule(this);"><i class="fa fa-trash-o"></i>&nbsp;Delete</a></td>' +
                '</tr>'
    };

    sportjuicecontrol.settings = function () {
        return settings;
    };

    sportjuicecontrol.init = function (options) {
        settings = $.extend(settings, options);

        //add item to list
        $.each(settings.juices, function (index, item) {
            addItemJuice(item);
        });

    };

    sportjuicecontrol.addRule = function () {
        addItemJuice();
    };

    var addItemJuice = function (juice) {
        //var appendedTag = settings.itemDOM.replace(/\[FeeType\]/g, '').replace('[FeeTypeName]', '').replace(/\[FeeUnit\]/g, '')
        //        .replace(/\[FeePrice\]/g, '').replace('[FeeTotal]', '').replace(/\[Description\]/g, '');

        $(settings.tbJuiceId + ' tbody').append(settings.itemDOM);

        $(settings.tbJuiceId + ' tbody tr:last select.sport').empty();
        $.each(settings.sports, function (index, item) {
            $(settings.tbJuiceId + ' tbody tr:last select.sport').append($('<option />').attr('value', item.Value + ' - ' + item.Description)
                .attr('data-sport', item.Value).attr('data-gametype', item.Description).text(item.Name));
        });

        $(settings.tbJuiceId + ' tbody tr:last select.bettype').empty();
        $.each(settings.betTypes, function (index, item) {
            $(settings.tbJuiceId + ' tbody tr:last select.bettype').append($('<option />').attr('value', item.Value).text(item.Name));
        });

        //bind data
        if (juice && juice != null)
        {
            var $this = $(settings.tbJuiceId + ' tbody tr:last');
            //console.log(juice);
            setOptionInsentitiveValue($this.find('select.sport'), juice.Sport + ' - ' + juice.GameType);
            $this.find('select.bettype').val(juice.BetType);
            $this.find('input:text').val(juice.Value);
            $this.find('input.gm:checkbox').attr('data-jid', juice.GmId).attr('data-cate', juice.GmCate).prop("checked", juice.GM);
            $this.find('input.1h:checkbox').attr('data-jid', juice.FhId).attr('data-cate', juice.FhCate).prop("checked", juice.FH);
            $this.find('input.2h:checkbox').attr('data-jid', juice.ShId).attr('data-cate', juice.ShCate).prop("checked", juice.SH);
        }
    };

    var setOptionInsentitiveValue = function ($selector, value) {
        var matchingValue = $selector.find('option').filter(function () {
            return this.value.toLowerCase() == value.toLowerCase();
        }).attr('value');
        $selector.val(matchingValue);

        //console.log(value + " : " + matchingValue);
    };

    sportjuicecontrol.deleteRule = function (sender) {
        if (confirm("are your really want to delete this rule?"))
        {
            $(sender).closest('tr').hide().find('input:checkbox').attr('data-cate', '');
        }
    };

    sportjuicecontrol.save = function () {
        if ($(settings.tbJuiceId + ' tbody tr:visible select.bettype option:selected').length != $(settings.tbJuiceId + ' tbody tr:visible select.bettype').length
            || $(settings.tbJuiceId + ' tbody tr:visible select.sport option:selected').length != $(settings.tbJuiceId + ' tbody tr:visible select.sport').length)
        {
            alert('Sport and Bet Type cannot be empty');
            return;
        }

        var rules = getRules();

        if (rules != null)
        {
            $.ajax({
                type: "POST",
                url: settings.saveURL,
                data: JSON.stringify(rules),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d) {
                        //alert('Success');
                        sportjuicecontrol.reloadPage();
                    }
                    else {
                        alert('Failed to save');
                    }
                },
                failure: function (response) {
                    alert('Failed to save');
                }
            });
        }
    };

    sportjuicecontrol.reloadPage = function () {
        location.reload();
    };

    var getRules = function () {
        var rules = [];
        $(settings.tbJuiceId + ' tbody tr').each(function () {
            $this = $(this);

            var chkGm = $this.find('input.gm:checkbox');
            var chkFh = $this.find('input.1h:checkbox');
            var chkSh = $this.find('input.2h:checkbox');

            var $selectedSport = $this.find('select.sport option:selected');

            var rule = {
                Sport: $selectedSport.data('sport'),
                GameType: $selectedSport.data('gametype'),
                BetType: $this.find('select.bettype').val(),
                Value: parseFloat($this.find('input:text').val()),
                GM: chkGm.is(':checked'),
                GmId: chkGm.data('jid'),
                GmCate: chkGm.data('cate'),
                FH: chkFh.is(':checked'),
                FhId: chkFh.data('jid'),
                FhCate: chkFh.data('cate'),
                SH: chkSh.is(':checked'),
                ShId: chkSh.data('jid'),
                ShCate: chkSh.data('cate'),
                RowIndex: $this.index()
            };

            rules.push(rule);
        });

        if (rules.length > 0) {
            return { rules: rules, agentId: $(settings.tbJuiceId).attr('agentid') };
        }

        return null;
    };

}(window.sportjuicecontrol = window.sportjuicecontrol || {}, jQuery));
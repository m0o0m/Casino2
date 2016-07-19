Type.registerNamespace("WebsiteLibrary");

WebsiteLibrary.CTabContainer = function(element) {
    WebsiteLibrary.CTabContainer.initializeDSL(this, [element]);
    
    this._headerCover = null;
    this._headerScroll = null;
    this._headerLeft = null;
    this._headerRight = null;
    this._headerCenter = null;
    this._headerHPad = null;
    this._header_onload$delegate = Function.createDelegate(this, this._header_onload);
    this._headerLeft_onclick$delegate = Function.createDelegate(this, this._headerLeft_onclick);
    this._headerRight_onclick$delegate = Function.createDelegate(this, this._headerRight_onclick);
}

WebsiteLibrary.CTabContainer.prototype = {
    initialize: function() {
        WebsiteLibrary.CTabContainer.callDSLMethod(this, "initialize");

        var body = $get(this.get_id() + "_body");
        var header = $get(this.get_id() + "_header");
        
        this._headerHPad = $get(this.get_id() + "_headerHPad");
        this._headerScroll = $get(this.get_id() + "_headerScroll");
        this._headerCover = $get(this.get_id() + "_headerCover");
        this._headerCenter = $get(this.get_id() + "_headerCenter");
        this._headerLeft = $get(this.get_id() + "_headerLeft");
        this._headerRight = $get(this.get_id() + "_headerRight");

        if ($common.getSize(this._headerScroll).width <= $common.getSize(body).width) return;

        header.style.width = this._headerCover.style.width = Sys.UI.DomElement.getBounds(body).width + 'px';
        Sys.UI.DomElement.addCssClass(this._headerHPad, "ajax__tab_headerHPad");
        Sys.UI.DomElement.addCssClass(this._headerCover, "ajax__tab_headerCover");
        Sys.UI.DomElement.addCssClass(this._headerCenter, "ajax__tab_headerCenter");
        Sys.UI.DomElement.addCssClass(this._headerLeft, "ajax__tab_headerLeft");
        Sys.UI.DomElement.addCssClass(this._headerRight, "ajax__tab_headerRight");
        
        this._headerCenter.style.position = "absolute"
        this._headerScroll.style.position = "absolute"
        $common.setLocation(this._headerScroll, new Sys.UI.Point(0, 0));
        
        Sys.Application.add_load(this._header_onload$delegate);
        $addHandlers(this._headerLeft, {
            click:this._headerLeft_onclick$delegate
        });
        $addHandlers(this._headerRight, {
            click:this._headerRight_onclick$delegate
        });
    },
    dispose : function() {
        Sys.Application.remove_load(this._header_onload$delegate);
        WebsiteLibrary.CTabContainer.callDSLMethod(this, "dispose");
    },
    _header_onload : function(sender, e) {
        var body = $get(this.get_id() + "_body");
        var hpad = $get(this.get_id() + "_headerHPad");
        var width = Sys.UI.DomElement.getBounds(body).width - Sys.UI.DomElement.getBounds(hpad).width;
        this._headerCenter.style.width = width + 'px';
    },
    _headerLeft_onclick : function(sender, e) {
        var outer = $common.getLocation(this._headerCenter);
        var inner = $common.getLocation(this._headerScroll);
        var x = inner.x - outer.x;
        if (x + 100 > 0) return;
        $common.setLocation(this._headerScroll, new Sys.UI.Point(x + 100, 0));
    },
    _headerRight_onclick : function(sender, e) {
        var outer = $common.getLocation(this._headerCenter);
        var inner = $common.getLocation(this._headerScroll);
        var x = inner.x - outer.x;
        if (x + $common.getSize(this._headerScroll).width < $common.getSize(this._headerCenter).width) return;
        $common.setLocation(this._headerScroll, new Sys.UI.Point(x - 100, 0));
    }
}

WebsiteLibrary.CTabContainer.registerClass('WebsiteLibrary.CTabContainer', AjaxControlToolkit.TabContainer);
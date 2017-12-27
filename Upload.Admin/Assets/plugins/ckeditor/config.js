/**
 * @license Copyright (c) 2003-2017, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    // config.uiColor = '#AADC6E';
    config.allowedContent = true;

    config.htmlEncodeOutput = true;

    config.syntaxhighlight_lang = 'csharp';
    config.syntaxhighlight_hideControls = true;
    config.language = 'vi';

    config.filebrowserBrowseUrl = '/Assets/plugins/ckfinder/ckfinder.html';
    config.filebrowserImageBrowseUrl = '/Assets/plugins/ckfinder.html?Type=Images';
    config.filebrowserFlashBrowseUrl = '/Assets/plugins/ckfinder.html?Type=Flash';

    config.filebrowserUploadUrl = '/Assets/plugins/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files';
    config.filebrowserImageUploadUrl = '/Uploaded';
    config.filebrowserFlashUploadUrl = '/Assets/plugins/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Flash';

    //config.extraPlugins = 'video';
    //config.extraPlugins = 'html5video';
    config.removePlugins = 'about';
    config.skin = 'kama';
    CKFinder.setupCKEditor(null, '/Assets/plugins/ckfinder/');
};

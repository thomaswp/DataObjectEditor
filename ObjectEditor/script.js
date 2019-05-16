function toggle(me) {
	var style = me.nextSibling.nextSibling.style;
	if (style.height != 'auto') {
		style.height = 'auto';
	} else {
		style.height = '0px';
	}
}
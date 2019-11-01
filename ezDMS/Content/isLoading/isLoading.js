
var m_area = null;

isloading = {

    start: function (p_area) {       
        var deferred = $.Deferred();
        if (document.getElementById('wfLoading')) {
            return;
        }
        var ele = document.createElement('div');
        ele.setAttribute('id', 'wfLoading');
        ele.classList.add('loading-layer');
        ele.innerHTML = '<span class="loading-wrap"><span class="loading-text"><span>.</span><span>.</span><span>.</span></span></span>';

        m_area = p_area === undefined ? m_area = document.body : m_area = p_area;

        if (p_area === undefined) {
            m_area = document.body;
            ele.style.position = "fixed";
        } else {
            m_area = p_area;
            ele.style.position = "relative";
        }

        //document.body.appendChild(ele);
        m_area.appendChild(ele);
        // Animation
        ele.classList.add('active-loading');
        deferred.resolve();
        return deferred.promise();
    },
    stop: function () {
        var ele = document.getElementById('wfLoading');
        if (ele) {
            //document.body.removeChild(ele);
            if (m_area != undefined || m_area != null) {
                m_area.removeChild(ele);
            }
        }
    }
}
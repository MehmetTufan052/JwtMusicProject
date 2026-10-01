(function () {
    var persistentAudio = null;

    function ensurePersistentAudio() {
        var placeholder = document.getElementById('audio');
        if (!persistentAudio) {
            if (placeholder) { persistentAudio = placeholder; }
            return;
        }
        if (placeholder && placeholder !== persistentAudio) {
            placeholder.replaceWith(persistentAudio);
        }
    }

    function clearKnownTimers() {
        if (window.timer) { clearInterval(window.timer); window.timer = null; }
    }

    function runScripts(container) {
        var scripts = Array.prototype.slice.call(container.querySelectorAll('script'));
        scripts.forEach(function (old) {
            var fresh = document.createElement('script');
            for (var i = 0; i < old.attributes.length; i++) {
                fresh.setAttribute(old.attributes[i].name, old.attributes[i].value);
            }
            fresh.textContent = old.textContent;
            old.parentNode.replaceChild(fresh, old);
        });
    }

    function swapTo(html, url, push) {
        ensurePersistentAudio();

        var doc = new DOMParser().parseFromString(html, 'text/html');

        clearKnownTimers();
        document.title = doc.title;
        document.body.innerHTML = doc.body.innerHTML;

        ensurePersistentAudio();
        runScripts(document.body);

        if (push) { history.pushState({ spa: true }, '', url); }
        window.scrollTo(0, 0);
    }

    window.spaNavigate = function (url, push) {
        push = push !== false;
        fetch(url, { headers: { 'X-Spa-Nav': '1' } })
            .then(function (res) {
                if (!res.ok) { window.location.href = url; return null; }
                return res.text();
            })
            .then(function (html) {
                if (html) { swapTo(html, url, push); }
            })
            .catch(function () { window.location.href = url; });
    };

    function isInternalLink(a) {
        if (!a) return false;
        if (a.target === '_blank') return false;
        if (a.hasAttribute('download')) return false;
        if (a.dataset.noRoute !== undefined) return false;
        var href = a.getAttribute('href');
        if (!href || href.charAt(0) !== '/') return false;
        if (href.indexOf('//') === 0) return false;
        return true;
    }

    document.addEventListener('click', function (e) {
        if (e.defaultPrevented || e.button !== 0) { return; }
        if (e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) { return; }
        var a = e.target.closest('a');
        if (!isInternalLink(a)) { return; }
        e.preventDefault();
        window.spaNavigate(a.getAttribute('href'));
    });

    window.addEventListener('popstate', function () {
        window.spaNavigate(location.pathname + location.search, false);
    });
})();

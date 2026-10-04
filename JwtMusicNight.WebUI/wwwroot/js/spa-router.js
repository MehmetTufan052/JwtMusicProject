(function () {
    var persistentAudio = null;
    var nowPlaying = { name: '', artist: '' };

    function fmtTime(s) {
        if (!isFinite(s) || s < 0) { s = 0; }
        var m = Math.floor(s / 60), r = Math.floor(s % 60);
        return m + ':' + (r < 10 ? '0' + r : r);
    }

    function syncNowPlayingUI() {
        var audio = persistentAudio;
        if (!audio) { return; }

        var nameEls = [document.getElementById('npName'), document.getElementById('nowName')].filter(Boolean);
        var subEls = [document.getElementById('npSub'), document.getElementById('nowArtist')].filter(Boolean);
        if (nowPlaying.name) { nameEls.forEach(function (el) { el.textContent = nowPlaying.name; }); }
        if (nowPlaying.artist) { subEls.forEach(function (el) { el.textContent = nowPlaying.artist; }); }

        var icons = [document.getElementById('plIcon'), document.getElementById('mainIcon')].filter(Boolean);
        if (audio.src) {
            icons.forEach(function (el) { el.className = audio.paused ? 'bi bi-play-fill' : 'bi bi-pause-fill'; });
        }

        var eq = document.getElementById('eq');
        var npArt = document.getElementById('npArt');
        if (eq) { eq.classList.toggle('on', !audio.paused && !!audio.src); }
        if (npArt) { npArt.classList.toggle('spin', !audio.paused && !!audio.src); }

        var fill = document.getElementById('trackFill');
        var cur = document.getElementById('tCur');
        var end = document.getElementById('tEnd');
        if (fill && audio.duration) { fill.style.width = Math.round((audio.currentTime / audio.duration) * 100) + '%'; }
        if (cur) { cur.textContent = fmtTime(audio.currentTime); }
        if (end && audio.duration) { end.textContent = fmtTime(audio.duration); }
    }

    window.spaSetNowPlaying = function (name, artist) {
        if (name) { nowPlaying.name = name; }
        if (artist) { nowPlaying.artist = artist; }
        syncNowPlayingUI();
    };

    function ensurePersistentAudio() {
        var placeholder = document.getElementById('audio');
        if (!persistentAudio) {
            if (placeholder) {
                persistentAudio = placeholder;
                ['play', 'pause', 'timeupdate', 'loadedmetadata'].forEach(function (evt) {
                    persistentAudio.addEventListener(evt, syncNowPlayingUI);
                });
            }
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
        document.head.innerHTML = doc.head.innerHTML;
        document.body.innerHTML = doc.body.innerHTML;

        ensurePersistentAudio();
        runScripts(document.body);
        syncNowPlayingUI();

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

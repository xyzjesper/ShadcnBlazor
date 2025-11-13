window.testy = {
    getBoundingBox: function (element) {
        return element.getBoundingClientRect();
    },
    
    getViewport: function () {
        return {
            height: window.innerHeight,
            width: window.innerWidth
        }
    }
}
mergeInto(LibraryManager.library, {

GetDeviceType: function() {
    var isMobile = /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);
    return isMobile ? 1 : 0;
  },

  // Returns URL query parameters as JSON string
  // string return from .jslib must be UTF8 pointer
  GetURLParameters: function() {
    try {
      var params = new URLSearchParams(window.location.search);
      var obj = {};
      params.forEach(function(value, key) { obj[key] = value; });
      var json = JSON.stringify(obj);
      return stringToNewUTF8(json);
    } catch (e) {
      return stringToNewUTF8("{}");
    }
  },

  // Alias for sending arbitrary game state (base64/snapshot)
  SendGameState: function(base64Ptr) {
    var base64 = UTF8ToString(base64Ptr);
    parent.postMessage({
      type: 'game_state',
      payload: { state: base64 }
    }, '*');
  },

  // Notify host that game is ready
  SendGameReady: function() {
    parent.postMessage({
      type: 'game_ready',
      payload: { ready: true }
    }, '*');
  },

  // Simple mobile check for webpages
  IsMobileWeb: function() {
    var isMobile = /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);
    return isMobile ? 1 : 0;
  },

    SendMatchResult: function(matchIdPtr, playerIdPtr, opponentIdPtr, outcomePtr, score, opponentScore, regionPtr) {
    var matchId = UTF8ToString(matchIdPtr);
    var playerId = UTF8ToString(playerIdPtr);
    var opponentId = UTF8ToString(opponentIdPtr);
    var outcome = UTF8ToString(outcomePtr);
    var region = UTF8ToString(regionPtr);

    parent.postMessage({
      type: 'match_result',
      payload: {
        matchId: matchId,
        playerId: playerId,
        opponentId: opponentId,
        outcome: outcome,
        score: score,
        opponentScore : opponentScore,
        region: region
      }
    }, '*');
  },

  SendMatchAbort: function(messagePtr, errorPtr, errorCodePtr) {
    var message = UTF8ToString(messagePtr);
    var error = UTF8ToString(errorPtr);
    var errorCode = UTF8ToString(errorCodePtr);

    parent.postMessage({
      type: 'match_abort',
      payload: {
        message: message,
        error: error,
        errorCode: errorCode
      }
    }, '*');
  },

  SendScreenshot: function(base64Ptr) {
    var base64 = UTF8ToString(base64Ptr);
    parent.postMessage({
      type: 'game_state',
      payload: {
        state: base64
      }
    }, '*');
  },

   SendBuildVersion: function(versionPtr) {
    var version = UTF8ToString(versionPtr);
    parent.postMessage({
      type: 'build_version',
      payload: {
        version: version
      }
    }, '*');
  }
});



var IFrameBridge = {
	GetURLParameters: function() {
		try {
			var params = new URLSearchParams(window.location.search);
			var data = {
				matchId: params.get('matchId') || params.get('matchid') || '',
				playerId: params.get('playerId') || params.get('playerid') || '',
				opponentId: params.get('opponentId') || params.get('opponentid') || ''
			};
			return stringToNewUTF8(JSON.stringify(data));
		} catch (e) { return stringToNewUTF8(''); }
	},
	SendMatchResult: function(matchIdPtr, playerIdPtr, opponentIdPtr, outcomePtr, score, opponentScore) {
		var matchId = UTF8ToString(matchIdPtr);
		var playerId = UTF8ToString(playerIdPtr);
		var opponentId = UTF8ToString(opponentIdPtr);
		var outcome = UTF8ToString(outcomePtr);
		try {
			window.parent && window.parent.postMessage && window.parent.postMessage({
				type: 'match_result',
				payload: { matchId: matchId, playerId: playerId, opponentId: opponentId, outcome: outcome, score: score|0, opponentScore: opponentScore|0 }
			}, '*');
		} catch (e) {}
	},
	SendMatchAbort: function(messagePtr, errorPtr, errorCodePtr) {
		var message = UTF8ToString(messagePtr);
		var error = UTF8ToString(errorPtr);
		var errorCode = UTF8ToString(errorCodePtr);
		try {
			window.parent && window.parent.postMessage && window.parent.postMessage({
				type: 'match_abort',
				payload: { message: message||'', error: error||'', errorCode: errorCode||'' }
			}, '*');
		} catch (e) {}
	},
	SendGameState: function(statePtr) {
		var state = UTF8ToString(statePtr);
		try {
			window.parent && window.parent.postMessage && window.parent.postMessage({
				type: 'game_state',
				payload: { state: state||'' }
			}, '*');
		} catch (e) {}
	},
	SendGameReady: function() {
		try { window.parent && window.parent.postMessage && window.parent.postMessage({ type: 'game_ready' }, '*'); } catch (e) {}
	},
	SendBuildVersion: function(versionPtr) {
		var version = UTF8ToString(versionPtr);
		try { window.parent && window.parent.postMessage && window.parent.postMessage({ type: 'build_version', payload: { version: version||'' } }, '*'); } catch (e) {}
	},
	IsMobileWeb: function() {
		try { return (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i).test(navigator.userAgent) ? 1 : 0; } catch (e) { return 0; }
	},
	GetDeviceType: function() {
		try { return (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i).test(navigator.userAgent) ? 1 : 0; } catch (e) { return 0; }
	}
};

(function(){
	if (typeof window === 'undefined') return;
	window.addEventListener('message', function(event){
		try {
			var data = event.data || {};
			var type = data.type || '';
			if (type === 'score_submitted' || type === 'match_result_ack') {
				if (typeof SendMessage === 'function') SendMessage('IFrameBridge', 'OnScoreSubmitted', '');
			} else if (type === 'pause') {
				if (typeof SendMessage === 'function') SendMessage('IFrameBridge', 'OnGamePaused', '');
			} else if (type === 'resume') {
				if (typeof SendMessage === 'function') SendMessage('IFrameBridge', 'OnGameResumed', '');
			} else if (type === 'connection_lost') {
				if (typeof SendMessage === 'function') SendMessage('IFrameBridge', 'OnConnectionLost', '');
			}
		} catch (e) {}
	});
})();

mergeInto(LibraryManager.library, IFrameBridge);
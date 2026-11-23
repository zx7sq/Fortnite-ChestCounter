namespace ChestCounter;

/// <summary>
/// The page of the graphical interface: one HTML file (styles and script inside), drawn by the script from the state
/// the counter sends (<see cref="GuiServer"/>). Texts come with the state, in the chosen language.
/// </summary>
public static class GuiPage
{
    private static Dictionary<string, string>? _cachedTexts;

    /// <summary>Every text of the page (cached, English-only).</summary>
    public static Dictionary<string, string> Texts() => _cachedTexts ??= new()
    {
        ["home"] = L.T("Accueil", "Home", "Inicio"),
        ["route"] = "Route",
        ["sprites"] = "Sprites",
        ["stats"] = "Stats",
        ["report"] = L.T("Rapport", "Report", "Informe"),
        ["overlay"] = L.T("Overlay", "Overlay", "Overlay"),
        ["settings"] = L.T("Réglages", "Settings", "Ajustes"),
        ["help"] = L.T("Aide", "Help", "Ayuda"),
        ["total"] = L.T("Coffres au total", "Chests in total", "Cofres en total"),
        ["totalSub"] = L.T("ouverts ou détruits par toi", "opened or destroyed by you", "abiertos o destruidos por ti"),
        ["byHand"] = L.T("dont {0} ajoutés à la main", "{0} of them added by hand", "de ellos, {0} añadidos a mano"),
        ["session"] = L.T("Session", "Session", "Sesión"),
        ["last"] = L.T("Dernière partie", "Last match", "Última partida"),
        ["squad"] = L.T("Total d'équipe", "Squad total", "Total del equipo"),
        ["squadSub"] = L.T("toi + {0} par tes coéquipiers", "you + {0} by your teammates", "tú + {0} de tus compañeros"),
        ["inaccurate"] = L.T("imprécis", "inaccurate", "impreciso"),
        ["inaccurateWhy"] = L.T("Compté dans TES replays : un coffre ouvert par un coéquipier à plus de 150 m de toi n'y est pas, et rien n'est enregistré après ton départ de la partie. Le vrai total de l'équipe est plus haut.",
            "Counted in YOUR replays: a chest a teammate opens more than 150 m away from you is not in them, and nothing is recorded once you leave the match. The real squad total is higher.",
            "Contado en TUS repeticiones: un cofre que un compañero abre a más de 150 m de ti no está en ellas, y no se graba nada después de que salgas de la partida. El total real del equipo es mayor."),
        ["treasures"] = L.T("Trésors", "Treasures", "Tesoros"),
        ["matches"] = L.T("parties", "matches", "partidas"),
        ["perMatch"] = L.T("{0} par partie", "{0} per match", "{0} por partida"),
        ["perDay"] = L.T("coffres / jour", "chests / day", "cofres / día"),
        ["perWeek"] = L.T("coffres / semaine", "chests / week", "cofres / semana"),
        ["openReport"] = L.T("Ouvrir le rapport", "Open the report", "Abrir el informe"),
        ["showOverlay"] = L.T("Afficher l'overlay", "Show the overlay", "Mostrar el overlay"),
        ["hideOverlay"] = L.T("Masquer l'overlay", "Hide the overlay", "Ocultar el overlay"),
        ["lock"] = L.T("Verrouiller l'overlay", "Lock the overlay", "Bloquear el overlay"),
        ["unlock"] = L.T("Déverrouiller pour le déplacer", "Unlock to move it", "Desbloquear para moverlo"),
        ["activity"] = L.T("Activité", "Activity", "Actividad"),
        ["noActivity"] = L.T("Rien pour l'instant.", "Nothing yet.", "Nada por ahora."),
        ["analyzing"] = L.T("Analyse de tes replays…", "Analyzing your replays…", "Analizando tus repeticiones…"),
        ["whatsNew"] = L.T("Nouveautés de la {0}", "What's new in {0}", "Novedades de la {0}"),
        ["newVersion"] = L.T("Nouvelle version {0} disponible", "New version {0} available", "Nueva versión {0} disponible"),
        ["install"] = L.T("Installer la mise à jour", "Install update", "Instalar actualización"),
        ["checkUpdate"] = L.T("Vérifier les mises à jour sur GitHub", "Check GitHub for updates", "Buscar actualizaciones en GitHub"),
        ["welcome"] = L.T("Bienvenue !", "Welcome!", "¡Bienvenido!"),
        ["welcomeText"] = L.T("Active d'abord les rediffusions dans Fortnite : Paramètres → Jeu → Rediffusions → Enregistrer les rediffusions. Ensuite, laisse le compteur ouvert : chaque partie est comptée quelques secondes après sa fin. Des coffres ouverts avant ? Le crayon de « Coffres au total » permet de les ajouter.",
            "First turn on replays in Fortnite: Settings → Game → Replays → Record Replays. Then keep the counter open: each match is counted a few seconds after it ends. Chests opened before? The pencil of \"Chests in total\" adds them.",
            "Primero activa las repeticiones en Fortnite: Ajustes → Juego → Repeticiones → Grabar repeticiones. Luego deja el contador abierto: cada partida se cuenta unos segundos después de terminar. ¿Cofres abiertos antes? El lápiz de «Cofres en total» permite añadirlos."),
        ["ok"] = L.T("OK", "OK", "OK"),
        ["moreInfo"] = L.T("En savoir plus", "More info", "Más información"),
        ["wMin"] = L.T("Réduire", "Minimize", "Minimizar"),
        ["wMax"] = L.T("Agrandir", "Maximize", "Maximizar"),
        ["wRestore"] = L.T("Niveau inférieur", "Restore down", "Restaurar"),
        ["wClose"] = L.T("Fermer", "Close", "Cerrar"),
        ["keepOpen"] = L.T("Garder le compteur ouvert", "Keep the counter open", "Mantener el contador abierto"),
        ["autoCloseText"] = L.T("Fortnite est fermé : le compteur, ouvert avec Fortnite, va se fermer tout seul.", "Fortnite is closed: the counter, opened with Fortnite, is about to close by itself.", "Fortnite está cerrado: el contador, abierto con Fortnite, se va a cerrar solo."),
        ["welcomeQuick"] = L.T("Tes premiers réglages (tu les retrouves à tout moment dans Réglages) :", "Your first settings (you find them at any time in Settings):", "Tus primeros ajustes (los encuentras en cualquier momento en Ajustes):"),
        ["welcomeDone"] = L.T("C'est parti !", "Let's go!", "¡Vamos!"),
        ["inGame"] = L.T("Overlay en jeu", "In-game overlay", "Overlay en el juego"),
        ["inGameHelp"] = L.T("Petite fenêtre toujours au premier plan qui affiche tes coffres en jeu (Fortnite en « Plein écran fenêtré »). Ctrl+Maj+O : afficher / masquer, Ctrl+Maj+L : verrouiller.",
            "Small always-on-top window that shows your chests in game (Fortnite in \"Windowed Fullscreen\"). Ctrl+Shift+O: show / hide, Ctrl+Shift+L: lock.",
            "Pequeña ventana siempre visible que muestra tus cofres en el juego (Fortnite en «Pantalla completa en ventana»). Ctrl+Mayús+O: mostrar / ocultar, Ctrl+Mayús+L: bloquear."),
        ["style"] = L.T("Style", "Style", "Estilo"),
        ["hud"] = L.T("HUD (intégré au jeu)", "HUD (blends into the game)", "HUD (integrado en el juego)"),
        ["classic"] = L.T("Classique (carte sombre)", "Classic (dark card)", "Clásico (tarjeta oscura)"),
        ["size"] = L.T("Taille", "Size", "Tamaño"),
        ["opacity"] = L.T("Opacité", "Opacity", "Opacidad"),
        ["squadToggle"] = L.T("Afficher le total d'équipe", "Show the squad total", "Mostrar el total del equipo"),
        ["squadHelp"] = L.T("Tes coffres + ceux de tous tes coéquipiers (même aléatoires), sur l'overlay et l'accueil.",
            "Your chests + those of all your teammates (random ones too), on the overlay and the home tab.",
            "Tus cofres + los de todos tus compañeros (también los aleatorios), en el overlay y el inicio."),
        ["obs"] = L.T("Overlay pour OBS (stream)", "Overlay for OBS (stream)", "Overlay para OBS (directo)"),
        ["obsSteps"] = L.T("Dans OBS : Sources → + → Navigateur, coche « Fichier local », choisis ce fichier. Largeur 360, hauteur 220 (450 × 300 en classique). Pour ne l'avoir que sur le stream, désactive l'overlay en jeu : la page continue d'être mise à jour.",
            "In OBS: Sources → + → Browser, tick \"Local file\", pick this file. Width 360, height 220 (450 × 300 for classic). To have it on the stream only, turn the in-game overlay off: the page keeps being updated.",
            "En OBS: Fuentes → + → Navegador, marca «Archivo local», elige este archivo. Ancho 360, alto 220 (450 × 300 en clásico). Para tenerlo solo en el directo, desactiva el overlay en el juego: la página se sigue actualizando."),
        ["copy"] = L.T("Copier le chemin", "Copy the path", "Copiar la ruta"),
        ["copied"] = L.T("Copié !", "Copied!", "¡Copiado!"),
        ["preview"] = L.T("Aperçu", "Preview", "Vista previa"),
        ["language"] = L.T("Langue", "Language", "Idioma"),
        ["times"] = L.T("Heures affichées", "Times shown in", "Horas mostradas en"),
        ["timesHelp"] = L.T("Dans le rapport et ici : semaines, heures du trésor, parties.", "In the report and here: weeks, Treasure Hours, matches.", "En el informe y aquí: semanas, horas del tesoro, partidas."),
        ["timesLocal"] = L.T("Heure locale ({0})", "Local time ({0})", "Hora local ({0})"),
        ["timesUtc"] = L.T("UTC (heure du jeu)", "UTC (game time)", "UTC (hora del juego)"),
        ["clock"] = L.T("Horloge", "Clock", "Reloj"),
        ["clockAuto"] = L.T("Comme Windows ({0})", "Same as Windows ({0})", "Como Windows ({0})"),
        ["clock12"] = L.T("12 heures (10:54 PM)", "12-hour (10:54 PM)", "12 horas (10:54 PM)"),
        ["clock24"] = L.T("24 heures (22:54)", "24-hour (22:54)", "24 horas (22:54)"),
        ["h12"] = L.T("12 h", "12-hour", "12 h"),
        ["h24"] = L.T("24 h", "24-hour", "24 h"),
        ["tray"] = L.T("Réduire dans la zone de notification", "Minimize to the notification area", "Minimizar al área de notificación"),
        ["trayHelp"] = L.T("Oui : réduire la fenêtre la range à côté de l'horloge, en bas à droite de la barre des tâches (icône du coffre, parfois derrière la petite flèche ^). Non : elle reste dans la barre des tâches comme une fenêtre normale. Dans les deux cas, le compteur continue de compter.",
            "Yes: minimizing the window puts it next to the clock, at the bottom right of the taskbar (chest icon, sometimes behind the small ^ arrow). No: it stays in the taskbar like a normal window. Either way, the counter keeps counting.",
            "Sí: minimizar la ventana la coloca junto al reloj, abajo a la derecha de la barra de tareas (icono del cofre, a veces detrás de la pequeña flecha ^). No: se queda en la barra de tareas como una ventana normal. En ambos casos, el contador sigue contando."),
        ["footerWindow"] = L.T("La croix de la fenêtre ferme le compteur (après confirmation).", "The close button of the window stops the counter (after asking).", "La cruz de la ventana cierra el contador (tras confirmarlo)."),
        ["withFortnite"] = L.T("Ouvrir avec Fortnite", "Open with Fortnite", "Abrir con Fortnite"),
        ["withFortniteHelp"] = L.T("Un petit processus invisible attend Fortnite et ouvre le compteur, qui se ferme après.", "A tiny invisible process waits for Fortnite and opens the counter, which closes afterwards.", "Un pequeño proceso invisible espera a Fortnite y abre el contador, que se cierra después."),
        ["zapMode"] = L.T("Mode Zap", "Zap mode", "Modo Zap"),
        ["zapModeHelp"] = L.T("Supprime les fichiers UnsavedReplay après comptage pour libérer de l'espace disque (les replays renommés sont conservés).", "Deletes UnsavedReplay files after counting to save disk space (manually saved replays are kept).", "Elimina los archivos UnsavedReplay después de contarlos para ahorrar espacio (las repeticiones guardadas se conservan)."),
        ["discordStatus"] = L.T("Statut Discord", "Discord status", "Estado de Discord"),
        ["discordStatusHelp"] = L.T("Affiche ta session actuelle sur ton profil Discord (nécessite un ID d'application depuis discord.com/developers/applications).", "Shows your current session on your Discord profile (requires an Application ID from discord.com/developers/applications).", "Muestra tu sesión actual en tu perfil de Discord (requiere un ID de aplicación de discord.com/developers/applications)."),
        ["discordAppId"] = L.T("ID d'application Discord", "Discord Application ID", "ID de aplicación de Discord"),
        ["saveId"] = L.T("Enregistrer l'ID", "Save ID", "Guardar ID"),
        ["treasureAlerts"] = L.T("Rappels des Heures du trésor", "Treasure Hour reminders", "Recordatorios de Horas del tesoro"),
        ["treasureAlertsHelp"] = L.T("Alerte dans l'app et notification Windows 5 min avant chaque Heure du trésor, et rappel au lancement de Fortnite le jour même.", "Shows an in-app banner and Windows pop-up 5 minutes before each Treasure Hour, plus a reminder when you boot Fortnite on Treasure Hour days.", "Muestra un aviso en la app y notificación de Windows 5 min antes de cada Hora del tesoro, y un recordatorio al abrir Fortnite ese día."),
        ["squadSync"] = "Squad Sync",
        ["squadSyncHelp"] = "Automatically detect squadmates running ChestCounter in the same match and count their exact chests in a separate counter.",
        ["thTitle"] = L.T("Heures du trésor", "Treasure Hours", "Horas del tesoro"),
        ["thLive"] = L.T("EN DIRECT", "LIVE NOW", "EN DIRECTO"),
        ["thNext"] = L.T("Prochaine dans", "Next in", "Próxima en"),
        ["thEnds"] = L.T("Se termine dans", "Ends in", "Termina en"),
        ["thToday"] = L.T("Aujourd'hui", "Today", "Hoy"),
        ["thLocal"] = L.T("Heures affichées dans ton fuseau horaire local", "Times shown in your local timezone", "Horas mostradas en tu zona horaria local"),
        ["updates"] = L.T("Mises à jour", "Updates", "Actualizaciones"),
        ["upPrompt"] = L.T("Proposées", "Offered", "Propuestas"),
        ["upAuto"] = L.T("Installées automatiquement", "Installed automatically", "Instaladas automáticamente"),
        ["upOff"] = L.T("Désactivées", "Off", "Desactivadas"),
        ["profile"] = L.T("Compte (profil)", "Account (profile)", "Cuenta (perfil)"),
        ["demos"] = L.T("Dossier des replays", "Replays folder", "Carpeta de repeticiones"),
        ["demosMissing"] = L.T("Dossier pas encore créé — joue une partie avec les rediffusions activées dans Fortnite.", "Folder not created yet — play one match with Replays enabled in Fortnite.", "Carpeta aún no creada — juega una partida con las repeticiones activadas en Fortnite."),
        ["save"] = L.T("Enregistrer le dossier", "Save folder", "Guardar carpeta"),
        ["editChests"] = L.T("Modifier mon nombre de coffres", "Edit my number of chests", "Modificar mi número de cofres"),
        ["close"] = L.T("Fermer", "Close", "Cerrar"),
        ["manual"] = L.T("Coffres ajoutés à la main", "Chests added by hand", "Cofres añadidos a mano"),
        ["manualHelp"] = L.T("Les coffres ouverts avant d'utiliser le compteur. Chaque nombre s'ajoute à ce que tu as déjà ajouté (un nombre négatif en retire) ; la croix annule un ajout.", "The chests opened before using the counter. Each number is added to what you already added (a negative number removes some); the cross undoes an addition.", "Los cofres abiertos antes de usar el contador. Cada número se suma a lo que ya añadiste (un número negativo quita cofres); la cruz anula un añadido."),
        ["manualTotal"] = L.T("Ajoutés à la main : {0}", "Added by hand: {0}", "Añadidos a mano: {0}"),
        ["addBtn"] = L.T("Ajouter", "Add", "Añadir"),
        ["removeEntry"] = L.T("Annuler cet ajout", "Undo this addition", "Anular este añadido"),
        ["thisWeek"] = L.T("Ouverts cette semaine", "Opened this week", "Abiertos esta semana"),
        ["thisWeekHelp"] = L.T("comptent aussi pour les trésors hebdo de la semaine {0}", "also count for the weekly treasures of week {0}", "cuentan también para los tesoros semanales de la semana {0}"),
        ["estimate"] = L.T("Ou estimer à partir de tes parties", "Or estimate from your matches", "O estimar a partir de tus partidas"),
        ["estimateHelp"] = L.T("Nombre de parties Battle Royale de la saison, dans Fortnite : Carrière → Statistiques.", "Number of Battle Royale matches this season, in Fortnite: Career → Stats.", "Número de partidas de Battle Royale de la temporada, en Fortnite: Carrera → Estadísticas."),
        ["career"] = L.T("Parties jouées", "Matches played", "Partidas jugadas"),
        ["compute"] = L.T("Calculer", "Compute", "Calcular"),
        ["add"] = L.T("Ajouter {0} coffres", "Add {0} chests", "Añadir {0} cofres"),
        ["estimateTooFew"] = L.T("Il faut au moins {0} parties comptées pour une moyenne fiable.", "At least {0} counted matches are needed for a reliable average.", "Hacen falta al menos {0} partidas contadas para una media fiable."),
        ["estimateNothing"] = L.T("Rien à ajouter : tes parties comptées couvrent déjà ce nombre.", "Nothing to add: your counted matches already cover that number.", "Nada que añadir: tus partidas contadas ya cubren ese número."),
        ["dataFolder"] = L.T("Ouvrir le dossier des données", "Open the data folder", "Abrir la carpeta de datos"),
        ["errorLog"] = L.T("Ouvrir le journal d'erreurs", "Open the error log", "Abrir el registro de errores"),
        ["issue"] = L.T("Signaler un problème (GitHub)", "Report a problem (GitHub)", "Informar de un problema (GitHub)"),
        ["project"] = L.T("Page du projet (GitHub)", "Project page (GitHub)", "Página del proyecto (GitHub)"),
        ["helpText"] = L.T("Le compteur lit les replays que Fortnite enregistre et compte tes coffres quelques secondes après chaque partie (Battle Royale de la saison seulement). Laisse-le ouvert pendant que tu joues : tu peux réduire la fenêtre, il continue de compter.",
            "The counter reads the replays Fortnite saves and counts your chests a few seconds after each match (the season's Battle Royale only). Keep it open while you play: you can minimize the window, it keeps counting.",
            "El contador lee las repeticiones que guarda Fortnite y cuenta tus cofres unos segundos después de cada partida (solo el Battle Royale de la temporada). Déjalo abierto mientras juegas: puedes minimizar la ventana, sigue contando."),
        ["uninstall"] = L.T("Désinstaller ChestCounter", "Uninstall ChestCounter", "Desinstalar ChestCounter"),
        ["keepData"] = L.T("Garder mon historique et mes réglages", "Keep my history and settings", "Conservar mi historial y mis ajustes"),
        ["deleteData"] = L.T("Tout supprimer", "Delete everything", "Borrarlo todo"),
        ["uninstallConfirm"] = L.T("Désinstaller maintenant ? Le compteur se ferme et ChestCounter.exe est supprimé.", "Uninstall now? The counter closes and ChestCounter.exe is deleted.", "¿Desinstalar ahora? El contador se cierra y se borra ChestCounter.exe."),
        ["quit"] = L.T("Quitter le compteur", "Quit the counter", "Salir del contador"),
        ["quitConfirm"] = L.T("Arrêter le compteur ? Les parties jouées ensuite ne seront comptées qu'à sa prochaine ouverture.", "Stop the counter? Matches played afterwards are only counted when it is opened again.", "¿Detener el contador? Las partidas jugadas después solo se cuentan cuando se vuelva a abrir."),
        ["closed"] = L.T("Le compteur est arrêté. Tu peux fermer cette fenêtre.", "The counter is stopped. You can close this window.", "El contador está detenido. Puedes cerrar esta ventana."),
        ["offline"] = L.T("Connexion au compteur perdue. Relance ChestCounter.exe pour te reconnecter.", "Lost connection to ChestCounter. Reopen ChestCounter.exe to reconnect.", "Se perdió la conexión con ChestCounter. Vuelve a abrir ChestCounter.exe para reconectar."),
        ["emptyMatches"] = L.T("Aucune partie comptée pour l'instant. Laisse ChestCounter ouvert pendant que tu joues à Fortnite : ta première partie apparaîtra ici quelques secondes après sa fin.", "No matches counted yet. Keep ChestCounter open while you play Fortnite — your first match will show up here a few seconds after it ends.", "Aún no hay partidas contadas. Mantén ChestCounter abierto mientras juegas a Fortnite: tu primera partida aparecerá aquí unos segundos después de terminar."),
        ["footer"] = L.T("Fermer cette fenêtre ne coupe pas le compteur.", "Closing this window does not stop the counter.", "Cerrar esta ventana no detiene el contador."),
    };

    public static string Html(string token) => Template.Replace("__TOKEN__", token).Replace("__FONT__", ReportWriter.FontFaceCss).Replace("__PENCIL__", ReportWriter.PencilIcon).Replace("__CHESTICON__", ReportWriter.ChestIconImg);

    private const string Template = """
        <!doctype html><html><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
        <title>Chest Counter</title>
        <style>
        :root{
          --bg:#0b0b0e;
          --bg-elevated:#141419;
          --ink:#f5f5f7;
          --muted:#8e8e93;
          --title:#ffffff;
          --card:#141419;
          --card-solid:#141419;
          --cink:#f5f5f7;
          --cmuted:#8e8e93;
          --line:rgba(255,255,255,.09);
          --line-strong:rgba(255,255,255,.18);
          --track:rgba(255,255,255,.08);
          --brand:#f59e0b;
          --brand-ink:#0b0b0e;
          --brand-soft:rgba(245,158,11,.14);
          --gold:#f59e0b;
          --team:#30d158;
          --warn:#ff453a;
          --accent:#f59e0b;
          --purple:#bf5af2;
          --s1:4px;--s2:8px;--s3:12px;--s4:16px;--s5:20px;--s6:24px;--s8:32px;
          --font:-apple-system,BlinkMacSystemFont,"SF Pro Display","SF Pro Text","Segoe UI Variable Display","Segoe UI",system-ui,sans-serif;
          --ease:cubic-bezier(.16,1,.3,1);
        }
        *{box-sizing:border-box;-webkit-font-smoothing:antialiased}
        @keyframes fadeUp{from{opacity:0;transform:translateY(8px)}to{opacity:1;transform:translateY(0)}}
        @keyframes modalIn{from{opacity:0;transform:scale(.96) translateY(6px)}to{opacity:1;transform:scale(1) translateY(0)}}
        @keyframes pulseGlow{0%,100%{transform:scale(1);opacity:1}50%{transform:scale(1.22);opacity:.72}}
        @keyframes spin{to{transform:rotate(360deg)}}
        body{
          margin:0;
          background:var(--bg);
          color:var(--ink);
          font:400 15px/1.55 var(--font);
          letter-spacing:-.011em;
          text-align:left;
        }
        main{max-width:1080px;margin:0 auto;padding:var(--s6) var(--s5) 36px;display:flex;flex-direction:column;gap:var(--s4);animation:fadeUp .38s var(--ease) both}
        .pirate{font-family:var(--font);font-weight:700;letter-spacing:-.025em}
        .num{font-family:var(--font);font-weight:700;font-variant-numeric:tabular-nums;letter-spacing:-.03em}
        .ico{width:16px;height:16px;display:inline-block;vertical-align:-3px;flex:none}
        .ico-lg{width:24px;height:24px;vertical-align:-4px}
        #app{display:flex;flex-direction:column;gap:var(--s5)}
        #hdr{display:flex;flex-direction:column;gap:var(--s4)}
        #view{display:flex;flex-direction:column;gap:var(--s5)}
        #view.tab-anim,#activityPane.tab-anim,#reportPane.tab-anim,#statsPane.tab-anim,#routePane.tab-anim{animation:fadeUp .3s var(--ease) both}
        #view:empty{display:none}
        #rep{width:100%;min-height:400px;border:0;display:block;background:transparent;overflow:hidden}
        .who{display:flex;align-items:center;gap:var(--s3);flex-wrap:wrap;color:var(--muted);font-size:13px;font-weight:500;margin-top:var(--s1)}
        .who select{min-height:30px;padding:3px var(--s3);font-size:13px;border-radius:999px;border:1px solid var(--line-strong);background:var(--bg-elevated);color:var(--ink);transition:border-color .2s var(--ease)}
        .who select:hover{border-color:var(--brand)}
        .who select option{color:#f5f5f7;background:#141419}
        header{display:flex;flex-wrap:wrap;align-items:center;justify-content:space-between;gap:var(--s4);border-bottom:1px solid var(--line);padding-bottom:var(--s5)}
        .brand{display:flex;align-items:center;gap:var(--s4)}
        .brand img,.brand svg{transition:transform .25s var(--ease)}
        .brand:hover img,.brand:hover svg{transform:scale(1.04)}
        h1{margin:0;font-size:32px;line-height:1.1;font-weight:700;letter-spacing:-.03em;color:var(--title)}
        .pill{display:inline-flex;align-items:center;gap:var(--s2);padding:7px var(--s4);border-radius:999px;background:var(--bg-elevated);border:1px solid var(--line);color:var(--ink);font-weight:600;font-size:13px;letter-spacing:-.01em;transition:border-color .2s var(--ease)}
        .pill i{width:8px;height:8px;border-radius:50%;background:#30d158}
        .pill.live i{background:#ff453a;animation:pulseGlow 1.6s ease-in-out infinite}
        .pill.busy i{background:var(--brand);animation:pulseGlow 1.6s ease-in-out infinite}
        .pill.warn i{background:#ff9f0a}
        nav{display:inline-flex;gap:var(--s1);flex-wrap:wrap;padding:var(--s1);border-radius:12px;background:var(--bg-elevated);border:1px solid var(--line);align-self:flex-start}
        nav button{min-height:36px;padding:6px var(--s4);border-radius:8px;border:1px solid transparent;background:transparent;color:var(--muted);font:600 14px var(--font);letter-spacing:-.01em;cursor:pointer;transition:color .2s var(--ease),background .2s var(--ease),transform .15s var(--ease)}
        nav button:hover{color:var(--ink);background:rgba(255,255,255,.05)}
        nav button:active{transform:scale(.97)}
        nav button.on{background:var(--brand);color:var(--brand-ink)}
        h2{margin:0 0 var(--s3);font-size:20px;line-height:1.25;font-weight:700;letter-spacing:-.02em;color:var(--title)}
        .tiles{display:grid;grid-template-columns:repeat(auto-fit,minmax(185px,1fr));gap:var(--s4)}
        .tile{position:relative;background:var(--card);color:var(--cink);border:1px solid var(--line);border-radius:16px;padding:var(--s5);transition:transform .22s var(--ease),border-color .22s var(--ease)}
        .tile:hover{transform:translateY(-2px);border-color:var(--line-strong)}
        .tile .v{font-size:32px;font-weight:700;line-height:1.1;color:#ffffff;letter-spacing:-.03em;display:flex;align-items:center;gap:var(--s2)}
        .tile .l{font-size:13px;color:var(--cmuted);margin-top:var(--s2);font-weight:500;line-height:1.4}
        .tile.hero{background:var(--bg-elevated);color:#ffffff;border:1px solid rgba(245,158,11,.38);padding:var(--s6)}
        .tile.hero:hover{border-color:var(--brand)}
        .editbtn{position:absolute;top:var(--s4);right:var(--s4);width:34px;height:34px;padding:0;border-radius:10px;border:1px solid var(--line-strong);background:rgba(255,255,255,.05);color:#ffffff;cursor:pointer;display:grid;place-items:center;transition:all .2s var(--ease)}
        .editbtn:hover{background:var(--brand);color:var(--brand-ink);border-color:var(--brand)}
        .editbtn:active{transform:scale(.95)}
        .editbtn:focus-visible{outline:2px solid var(--brand);outline-offset:2px}
        dialog{width:min(540px,calc(100% - 32px));padding:var(--s6);border:1px solid var(--line-strong);border-radius:18px;background:var(--bg-elevated);color:var(--cink);box-shadow:0 24px 64px rgba(0,0,0,.7);animation:modalIn .24s var(--ease) both}
        dialog::backdrop{background:rgba(0,0,0,.72)}
        .dhead{display:flex;align-items:center;justify-content:space-between;gap:var(--s3);margin-bottom:var(--s3)}
        .dhead h2{margin:0;color:#ffffff}
        .dclose{width:34px;height:34px;border-radius:10px;border:1px solid var(--line);background:rgba(255,255,255,.06);color:var(--cink);cursor:pointer;display:grid;place-items:center;transition:all .2s var(--ease)}
        .dclose:hover{background:rgba(255,255,255,.14)}
        .tile.hero .v{font-size:52px;color:#ffffff;margin:var(--s1) 0}
        .tile.hero .l{color:var(--cmuted)}
        .card{background:var(--card);color:var(--cink);border:1px solid var(--line);border-radius:16px;padding:var(--s5);transition:border-color .2s var(--ease)}
        .card:hover{border-color:var(--line-strong)}
        .empty-state{display:flex;align-items:flex-start;gap:var(--s4);padding:var(--s6);text-align:left}
        .empty-state .es-title{font-size:16px;font-weight:700;color:#ffffff;margin-bottom:var(--s1)}
        .empty-state .es-body{font-size:14px;color:var(--cmuted);line-height:1.5}
        .th-alert{border-left:4px solid var(--brand);background:var(--bg-elevated)}
        .th-card{display:flex;flex-direction:column;gap:var(--s4);border-color:rgba(245,158,11,.28)}
        .th-head{display:flex;align-items:center;justify-content:space-between;gap:var(--s3);flex-wrap:wrap}
        .th-title{display:flex;align-items:center;gap:var(--s2);font-size:17px;font-weight:700;color:#ffffff;margin:0}
        .th-cd{display:inline-flex;align-items:center;gap:var(--s2);padding:5px var(--s3);border-radius:999px;background:var(--brand-soft);border:1px solid rgba(245,158,11,.35);color:var(--brand);font-size:13px;font-weight:700;font-variant-numeric:tabular-nums}
        .th-cd.live{background:rgba(48,209,88,.15);border-color:rgba(48,209,88,.4);color:#30d158}
        .th-boosts{display:flex;flex-wrap:wrap;gap:var(--s2)}
        .th-boost{font-size:12px;font-weight:600;color:#f5f5f7;background:rgba(255,255,255,.05);border:1px solid var(--line);border-radius:999px;padding:4px 10px}
        .th-boost b{color:var(--brand)}
        .th-list{display:flex;flex-direction:column}
        .th-row{display:flex;align-items:center;justify-content:space-between;gap:var(--s3);padding:10px 0;border-top:1px solid var(--line);font-size:14px}
        .th-row .th-when{color:#ffffff;font-weight:600}
        .th-row .th-time{color:var(--cmuted);font-variant-numeric:tabular-nums}
        .th-pill{display:inline-block;margin-left:var(--s2);padding:2px 8px;border-radius:999px;font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:.04em;background:var(--brand-soft);color:var(--brand)}
        .th-pill.live{background:rgba(48,209,88,.18);color:#30d158}
        .th-foot{font-size:12px;color:var(--cmuted)}
        .gw-card{display:flex;flex-direction:column;gap:var(--s4);border-color:rgba(245,158,11,.42);background:linear-gradient(135deg,rgba(245,158,11,.10) 0%,var(--card) 58%)}
        .gw-sub{margin:0;font-size:13.5px;line-height:1.5;color:#d1d1d6}
        .gw-tags{display:flex;flex-wrap:wrap;gap:6px}
        .gw-tag{font-size:12px;font-weight:600;color:#fde68a;background:rgba(245,158,11,.12);border:1px solid rgba(245,158,11,.28);border-radius:999px;padding:4px 10px}
        .gw-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(200px,1fr));gap:10px}
        .gw-box{background:rgba(11,11,14,.55);border:1px solid var(--line);border-radius:12px;padding:10px 12px;display:flex;flex-direction:column;gap:3px}
        .gw-box .gl{font-size:11px;font-weight:700;text-transform:uppercase;letter-spacing:.05em;color:var(--cmuted)}
        .gw-box .gv{font-size:13.5px;font-weight:700;color:#ffffff;font-variant-numeric:tabular-nums}
        .spin{width:20px;height:20px;flex:none;margin-top:2px;border:2.5px solid var(--line-strong);border-top-color:var(--brand);border-radius:50%;animation:spin .8s linear infinite}
        .row{display:flex;align-items:center;justify-content:space-between;gap:var(--s4);padding:var(--s4) 0;border-bottom:1px solid var(--line)}
        .row:last-child{border-bottom:none}
        .row .t{font-size:15px;font-weight:600;color:#ffffff}.row .h{font-size:13px;color:var(--cmuted);margin-top:var(--s1);line-height:1.45}
        .actions{display:flex;flex-wrap:wrap;gap:var(--s3)}
        button.b{min-height:38px;padding:var(--s2) var(--s5);border-radius:10px;border:1px solid var(--line-strong);background:rgba(255,255,255,.05);color:var(--cink);font:600 14px var(--font);letter-spacing:-.01em;cursor:pointer;transition:all .18s var(--ease)}
        button.b:hover{background:rgba(255,255,255,.11);border-color:rgba(255,255,255,.28)}
        button.b:active{transform:scale(.97)}
        button.b.main{background:var(--brand);color:var(--brand-ink);border-color:var(--brand)}
        button.b.main:hover{filter:brightness(1.08)}
        button.b.danger{border-color:rgba(255,69,58,.4);color:#ff453a;background:rgba(255,69,58,.08)}
        button.b.danger:hover{background:rgba(255,69,58,.18);border-color:#ff453a}
        select,input[type=text],input[type=number]{min-height:38px;padding:6px var(--s3);border-radius:10px;border:1px solid var(--line-strong);background:rgba(255,255,255,.05);color:var(--cink);font:500 14px var(--font);transition:border-color .18s var(--ease)}
        select:hover,input[type=text]:hover,input[type=number]:hover{border-color:rgba(255,255,255,.28)}
        select:focus,input[type=text]:focus,input[type=number]:focus{outline:none;border-color:var(--brand);box-shadow:0 0 0 3px var(--brand-soft)}
        select option{background:#141419;color:#f5f5f7}
        input[type=text]{width:min(520px,100%)}
        .switch{position:relative;display:inline-block;width:50px;height:30px;flex:none;vertical-align:middle}
        .switch input{opacity:0;width:100%;height:100%;margin:0;cursor:pointer;position:absolute;z-index:1}
        .switch span{position:absolute;inset:0;border-radius:999px;background:rgba(255,255,255,.14);border:1px solid rgba(255,255,255,.08);transition:background .22s var(--ease),border-color .22s var(--ease)}
        .switch span:after{content:"";position:absolute;top:2px;left:2px;width:24px;height:24px;border-radius:50%;background:#ffffff;box-shadow:0 2px 6px rgba(0,0,0,.45);transition:transform .22s var(--ease)}
        .switch input:checked+span{background:var(--brand);border-color:var(--brand)}.switch input:checked+span:after{transform:translateX(20px)}
        .switch input:focus-visible+span{outline:2px solid var(--brand);outline-offset:2px}
        .badge{display:inline-block;margin-left:6px;padding:2px var(--s2);border-radius:999px;background:var(--brand-soft);border:1px solid rgba(245,158,11,.35);color:var(--brand);font-size:11px;font-weight:600;vertical-align:1px;cursor:help}
        .log{font:13px/1.6 "SF Mono","Cascadia Mono",Consolas,monospace;max-height:260px;overflow:auto;white-space:pre-wrap;background:#0f0f13;border-color:var(--line)}
        .log .err{color:#ff453a}.log .warn{color:#ffd60a}.log .ok{color:#30d158}.log .info{color:#64d2ff}.log .tre{color:#bf5af2}.log .mut{color:var(--cmuted)}
        .bar{height:8px;background:var(--track);border-radius:999px;overflow:hidden;margin-top:var(--s3)}.bar i{display:block;height:100%;border-radius:999px;background:var(--brand);transition:width .35s var(--ease)}
        .path{font:13px "SF Mono","Cascadia Mono",Consolas,monospace;word-break:break-all;background:rgba(255,255,255,.04);border:1px solid var(--line);padding:var(--s2) var(--s3);border-radius:10px;display:block;margin:var(--s2) 0;color:#d1d1d6}
        .note{font-size:13px;color:var(--muted)}
        .hidden{display:none!important}
        footer{display:flex;justify-content:space-between;align-items:center;gap:var(--s3);flex-wrap:wrap;border-top:1px solid var(--line);padding-top:var(--s4);color:var(--muted);font-size:13px}
        .estimate{margin-top:var(--s3);font-weight:600;color:#ffffff}
        .mtotal{font-weight:600;margin-bottom:var(--s2);color:#ffffff}
        .mweek{display:flex;align-items:center;gap:8px;margin-top:10px;cursor:pointer}
        .mweek input{accent-color:#ffffff;width:16px;height:16px}
        .mweek b{white-space:nowrap;color:#ffffff}
        .mweek span{color:var(--cmuted);font-size:13px}
        .mhist{list-style:none;margin:12px 0 0;padding:0;max-height:190px;overflow:auto}
        .mhist li{display:flex;align-items:center;gap:12px;padding:7px 4px;border-top:1px solid var(--line)}
        .mhist b{min-width:70px;text-align:right;color:#ffffff}
        .mhist span{flex:1;color:var(--cmuted);font-size:13px}
        .mhist .mx{border:0;background:none;color:var(--cmuted);font-size:18px;line-height:1;cursor:pointer;padding:2px 7px;border-radius:6px;transition:all .18s var(--ease)}
        .mhist .mx:hover{color:var(--warn);background:rgba(255,69,58,.14)}
        .notice{position:relative;display:flex;gap:12px;align-items:flex-start;border-left:4px solid #ffffff}
        .notice.warning{border-left-color:#ff453a;background:rgba(255,69,58,.08);border-color:rgba(255,69,58,.25)}
        .notice .ni{flex:none;width:24px;height:24px;border-radius:50%;display:grid;place-items:center;font:700 13px var(--font);background:#ffffff;color:#050507;margin-top:1px}
        .notice.warning .ni{background:#ff453a;color:#ffffff}
        .notice .nb{flex:1;min-width:0}.notice .nt{font-weight:700;margin-bottom:2px;color:#ffffff}.notice .nx{white-space:pre-line;color:#d1d1d6}
        .notice .actions{margin-top:10px}
        .notice .nclose{flex:none;width:30px;height:30px;margin:-4px -6px 0 0;border:0;border-radius:50%;background:transparent;color:var(--cmuted);font-size:18px;line-height:1;cursor:pointer;transition:all .18s var(--ease)}
        .notice .nclose:hover{background:rgba(255,255,255,.1);color:#ffffff}
        @media (max-width:640px){.row{flex-direction:column;align-items:flex-start}}
        /* The counter's own window: its title bar is drawn here (drag to move, double click to maximize). */
        .tbar{display:none}
        body.windowed{height:100vh;overflow:hidden}
        body.windowed .tbar{display:flex;justify-content:flex-end;position:fixed;top:0;left:0;right:0;height:34px;background:rgba(6,6,8,.85);backdrop-filter:blur(20px);border-bottom:1px solid rgba(255,255,255,.05);z-index:20;app-region:drag;user-select:none}
        body.windowed #scroll{position:absolute;top:34px;left:0;right:0;bottom:0;overflow:auto;scroll-behavior:smooth}
        body.windowed header{app-region:drag}
        body.windowed header select,body.windowed header button{app-region:no-drag}
        .wctl{display:flex;app-region:no-drag}
        .wctl button{width:46px;height:34px;padding:0;border:0;border-radius:0;background:transparent;color:var(--muted);display:grid;place-items:center;cursor:pointer;transition:background .15s,color .15s}
        .wctl button:hover{background:rgba(255,255,255,.1);color:#ffffff}
        .wctl button.x:hover{background:#ff453a;color:#ffffff}
        .wctl button:focus-visible{outline:2px solid #ffffff;outline-offset:-2px}
        .wctl svg{display:block}
        /* Scrollbars in the colors of the tool (no white bar on the side). */
        #scroll::-webkit-scrollbar,html::-webkit-scrollbar{width:10px}
        #scroll::-webkit-scrollbar-track,html::-webkit-scrollbar-track{background:transparent}
        #scroll::-webkit-scrollbar-thumb,html::-webkit-scrollbar-thumb{background:rgba(255,255,255,.18);border-radius:999px;border:2px solid var(--bg)}
        #scroll::-webkit-scrollbar-thumb:hover,html::-webkit-scrollbar-thumb:hover{background:rgba(255,255,255,.34)}
        .log::-webkit-scrollbar{width:8px}
        .log::-webkit-scrollbar-track{background:transparent}
        .log::-webkit-scrollbar-thumb{background:rgba(255,255,255,.16);border-radius:999px}
        /* Stats Card */
        #statsPane{display:flex;flex-direction:column;gap:12px}
        .gs-top{display:flex;align-items:center;justify-content:space-between;gap:10px;flex-wrap:wrap}
        .gs-search{display:flex;align-items:center;flex:1;min-width:260px;max-width:440px;height:38px;background:var(--card);border:1px solid var(--line-strong);border-radius:10px;padding:3px 3px 3px 8px;transition:border-color .18s var(--ease)}
        .gs-search:focus-within{border-color:var(--brand);box-shadow:0 0 0 3px var(--brand-soft)}
        .gs-search select{background:transparent;color:var(--brand);border:none;font:700 12.5px var(--font);min-height:30px;padding:0 6px;cursor:pointer;box-shadow:none}
        .gs-search select:focus{box-shadow:none;border:none}
        .gs-div{width:1px;height:18px;background:var(--line);margin:0 4px}
        .gs-search input{flex:1;width:auto;min-width:100px;min-height:30px;background:transparent;border:none;color:var(--ink);padding:0 8px;font:500 14px var(--font);box-shadow:none}
        .gs-search input:focus{box-shadow:none;border:none}
        .gs-btn-gold{height:30px;padding:0 14px;border-radius:7px;border:none;background:var(--brand);color:var(--brand-ink);font:700 13px var(--font);letter-spacing:-.01em;cursor:pointer;transition:filter .15s,transform .15s}
        .gs-btn-gold:hover{filter:brightness(1.08)}
        .gs-btn-gold:active{transform:scale(.97)}
        .gs-btn-gold:disabled{opacity:.55;cursor:wait}
        .gs-acts{display:flex;gap:8px;align-items:center;flex-wrap:wrap}
        .gs-ghost{display:inline-flex;align-items:center;gap:7px;height:38px;padding:0 14px;border-radius:10px;background:var(--card);color:var(--cmuted);border:1px solid var(--line);font:600 13px var(--font);letter-spacing:-.01em;cursor:pointer;transition:all .18s var(--ease)}
        .gs-ghost:hover{color:var(--ink);border-color:var(--line-strong);background:rgba(255,255,255,.06)}
        .gs-ghost.live-on{color:#30d158;border-color:rgba(48,209,88,.35);background:rgba(48,209,88,.1)}
        .gs-dot{width:7px;height:7px;border-radius:50%;background:var(--brand)}
        .gs-dot.ok{background:#30d158}
        .gs-ctrls{display:flex;flex-wrap:wrap;gap:8px;align-items:center}
        .gs-seg{display:inline-flex;gap:3px;padding:3px;border-radius:10px;background:var(--card);border:1px solid var(--line)}
        .gs-pill{height:30px;padding:0 12px;border-radius:7px;border:none;background:transparent;color:var(--cmuted);font:600 12.5px var(--font);letter-spacing:-.01em;cursor:pointer;transition:all .18s var(--ease)}
        .gs-pill:hover{color:var(--ink);background:rgba(255,255,255,.05)}
        .gs-pill.on{background:var(--brand);color:var(--brand-ink)}
        .gs-banner{padding:10px 14px;border-radius:12px;font-size:13.5px;font-weight:500;background:var(--brand-soft);border:1px solid rgba(245,158,11,.35);color:var(--ink)}
        .gs-banner.err{background:rgba(255,69,58,.1);border-color:rgba(255,69,58,.35);color:#ff6961}
        .gs-card{position:relative;display:grid;grid-template-columns:1.6fr .8fr;gap:20px;padding:22px 24px;border-radius:16px;background:var(--card);border:1px solid rgba(245,158,11,.26);transition:border-color .2s var(--ease);overflow:hidden}
        .gs-card:hover{border-color:rgba(245,158,11,.42)}
        @keyframes gsFlash{0%{border-color:var(--brand);box-shadow:0 0 0 3px var(--brand-soft)}100%{border-color:rgba(245,158,11,.26);box-shadow:none}}
        .gs-card.flash{animation:gsFlash 1.2s ease-out}
        .gs-left{display:flex;flex-direction:column;justify-content:space-between;gap:14px;min-width:0}
        .gs-meta{display:flex;align-items:center;flex-wrap:wrap;gap:8px;margin-bottom:4px}
        .gs-meta-gold{font-size:11.5px;font-weight:700;letter-spacing:.04em;color:var(--brand)}
        .gs-meta-dot{color:var(--cmuted);opacity:.5}
        .gs-meta-sub{font-size:11.5px;font-weight:600;letter-spacing:.03em;color:var(--cmuted)}
        .gs-src{margin-left:auto;font-size:11px;font-weight:700;letter-spacing:.04em;padding:3px 10px;border-radius:999px;text-transform:uppercase}
        .gs-src.demo{background:var(--brand-soft);border:1px solid rgba(245,158,11,.35);color:var(--brand)}
        .gs-src.live{background:rgba(48,209,88,.14);border:1px solid rgba(48,209,88,.35);color:#30d158}
        .gs-id{display:flex;align-items:center;justify-content:space-between;flex-wrap:wrap;gap:12px}
        .gs-name{font-family:var(--font);font-size:clamp(26px,3vw,32px);font-weight:700;line-height:1.1;letter-spacing:-.03em;color:#ffffff;margin:0;word-break:break-word}
        .gs-lvl{display:inline-flex;align-items:center;gap:8px;background:rgba(255,255,255,.04);border:1px solid var(--line);border-radius:999px;padding:5px 12px}
        .gs-lvl-l{font-size:11px;font-weight:700;letter-spacing:.04em;color:var(--brand)}
        .gs-lvl-v{font-family:var(--font);font-size:15px;font-weight:700;font-variant-numeric:tabular-nums;color:#ffffff}
        .gs-bp{width:56px;height:5px;background:var(--track);border-radius:999px;overflow:hidden}
        .gs-bp-f{height:100%;background:var(--brand);border-radius:999px;transition:width .35s var(--ease)}
        .gs-rows{display:flex;flex-direction:column;gap:8px}
        .gs-row{display:flex;align-items:center;border-radius:12px;padding:13px 16px;background:rgba(255,255,255,.03);border:1px solid var(--line);transition:border-color .18s var(--ease),background .18s var(--ease)}
        .gs-row:hover{background:rgba(255,255,255,.05);border-color:var(--line-strong)}
        .gs-rico{width:28px;height:28px;flex:none;margin-right:12px;display:flex;align-items:center;justify-content:center;color:var(--brand)}
        .gs-rico svg{width:20px;height:20px}
        .gs-cells{flex:1;display:grid;align-items:center}
        .gs-cells.c3{grid-template-columns:repeat(3,1fr)}
        .gs-cells.c2{grid-template-columns:repeat(2,1fr)}
        .gs-cell{text-align:center;padding:0 8px;position:relative}
        .gs-cell+.gs-cell::before{content:'';position:absolute;left:0;top:14%;bottom:14%;width:1px;background:var(--line)}
        .gs-val{font-family:var(--font);font-size:clamp(22px,2.3vw,26px);font-weight:700;line-height:1.15;letter-spacing:-.03em;color:#ffffff;display:inline-block;font-variant-numeric:tabular-nums}
        .gs-lbl{font-size:11px;font-weight:600;letter-spacing:.03em;margin-top:3px;color:var(--cmuted)}
        @keyframes gsRoll{0%{opacity:.4;transform:translateY(6px);color:var(--brand)}100%{opacity:1;transform:translateY(0);color:#ffffff}}
        .gs-roll{animation:gsRoll .45s var(--ease)}
        .gs-extra{display:grid;grid-template-columns:repeat(4,1fr);gap:8px;padding-top:12px;border-top:1px solid var(--line);text-align:center}
        .gs-ex{display:flex;flex-direction:column;align-items:center;gap:2px}
        .gs-ex-l{font-size:10.5px;font-weight:600;letter-spacing:.03em;color:var(--cmuted)}
        .gs-ex-v{font-family:var(--font);font-size:16px;font-weight:700;letter-spacing:-.02em;color:#ffffff;display:inline-block;font-variant-numeric:tabular-nums}
        .gs-right{position:relative;display:flex;flex-direction:column;align-items:center;justify-content:center;padding:16px 12px 34px;border-radius:12px;background:rgba(255,255,255,.02);border:1px solid var(--line);min-height:0}
        .gs-spot{position:absolute;width:210px;height:210px;top:46%;left:50%;transform:translate(-50%,-50%);border-radius:50%;background:radial-gradient(circle,rgba(245,158,11,.16) 0%,transparent 70%);filter:blur(18px);pointer-events:none}
        .gs-skin{position:relative;z-index:2;max-height:310px;width:100%;object-fit:contain;transition:transform .25s var(--ease)}
        .gs-skin:hover{transform:scale(1.03)}
        .gs-cap{position:absolute;bottom:10px;left:0;right:0;text-align:center;z-index:2;font-size:11px;font-weight:600;letter-spacing:.08em;color:var(--cmuted);text-transform:uppercase}
        .gs-skinbar{display:flex;align-items:center;justify-content:space-between;flex-wrap:wrap;gap:10px;padding:10px 14px;border-radius:14px;background:var(--card);border:1px solid var(--line)}
        .gs-presets{display:flex;align-items:center;flex-wrap:wrap;gap:6px}
        .gs-sbl{font-size:12px;font-weight:600;color:var(--cmuted);margin-right:4px}
        .gs-chip{height:30px;padding:0 11px;background:rgba(255,255,255,.04);color:var(--cmuted);border:1px solid var(--line);border-radius:8px;font:600 12.5px var(--font);cursor:pointer;transition:all .18s var(--ease)}
        .gs-chip:hover{color:var(--ink);border-color:var(--line-strong)}
        .gs-chip.on{background:var(--brand-soft);border-color:rgba(245,158,11,.4);color:var(--brand)}
        .gs-sform{display:flex;gap:6px;align-items:center}
        .gs-sform input{width:155px;height:30px;min-height:30px;padding:0 11px;border-radius:8px;background:rgba(255,255,255,.04);border:1px solid var(--line);font-size:13px}
        .gs-sform button{height:30px;min-height:30px;padding:0 12px;border-radius:8px}
        @media (max-width:860px){.gs-card{grid-template-columns:1fr;padding:18px}.gs-right{min-height:240px}.gs-skin{max-height:220px}}
        @media (max-width:560px){.gs-cells.c3{grid-template-columns:1fr;gap:10px}.gs-cell+.gs-cell::before{display:none}.gs-extra{grid-template-columns:repeat(2,1fr)}}
        /* Sprites Tab */
        .sp-bar{display:flex;flex-wrap:wrap;gap:8px;align-items:center;justify-content:space-between}
        .sp-grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(320px,1fr));gap:12px}
        .sp-card{background:var(--card);border:1px solid var(--line);border-radius:14px;padding:16px;display:flex;flex-direction:column;gap:12px;transition:border-color .18s var(--ease)}
        .sp-card:hover{border-color:var(--line-strong)}
        .sp-head{display:flex;align-items:center;gap:12px}
        .sp-thumb{width:46px;height:46px;border-radius:10px;background:rgba(255,255,255,.04);border:1px solid var(--line);object-fit:contain;padding:3px;flex:none}
        .sp-hinfo{flex:1;min-width:0}
        .sp-htop{display:flex;align-items:center;justify-content:space-between;gap:8px}
        .sp-name{font-size:15.5px;font-weight:700;color:#ffffff;letter-spacing:-.01em}
        .sp-ab{font-size:12px;color:var(--cmuted);line-height:1.35;margin-top:2px}
        .sp-rar{font-size:10.5px;font-weight:700;letter-spacing:.05em;text-transform:uppercase;padding:2px 8px;border-radius:999px;border:1px solid var(--line);color:var(--cmuted);flex:none}
        .sp-rar.mythic{color:#f59e0b;border-color:rgba(245,158,11,.4);background:rgba(245,158,11,.12)}
        .sp-rar.legendary{color:#fb923c;border-color:rgba(251,146,60,.4);background:rgba(251,146,60,.12)}
        .sp-rar.epic{color:#c084fc;border-color:rgba(192,132,252,.4);background:rgba(192,132,252,.12)}
        .sp-rar.rare{color:#38bdf8;border-color:rgba(56,189,248,.4);background:rgba(56,189,248,.12)}
        .sp-vars{display:grid;grid-template-columns:repeat(4,1fr);gap:6px}
        .sp-vars.single{grid-template-columns:minmax(0,110px)}
        .sp-var{display:flex;flex-direction:column;align-items:center;gap:4px;padding:8px 6px;border-radius:10px;background:rgba(255,255,255,.025);border:1px solid var(--line);color:var(--cmuted);font:600 11px var(--font);cursor:pointer;text-align:center;transition:all .16s var(--ease)}
        .sp-var img{width:46px;height:46px;object-fit:contain;opacity:.42;filter:grayscale(.45);transition:all .18s var(--ease)}
        .sp-var:hover{border-color:var(--line-strong);color:var(--ink);background:rgba(255,255,255,.05)}
        .sp-var:hover img{opacity:.85;filter:none;transform:scale(1.06)}
        .sp-var.owned{color:#ffffff;background:rgba(48,209,88,.1);border-color:rgba(48,209,88,.4)}
        .sp-var.owned img{opacity:1;filter:none}
        .sp-var.v-fortune.owned{color:#7dd3fc;background:rgba(56,189,248,.12);border-color:rgba(56,189,248,.45)}
        .sp-var.v-cursed.owned{color:#fca5a5;background:rgba(255,69,58,.12);border-color:rgba(255,69,58,.45)}
        .sp-var.v-gold.owned{color:#fcd34d;background:rgba(245,158,11,.14);border-color:rgba(245,158,11,.5)}
        .sp-tags{display:flex;flex-wrap:wrap;justify-content:center;gap:3px;min-height:15px}
        .sp-tag{font-size:9px;font-weight:700;padding:1px 5px;border-radius:5px;letter-spacing:.03em;text-transform:uppercase;background:rgba(255,255,255,.08);color:var(--ink)}
        .sp-tag.won{background:rgba(48,209,88,.2);color:#30d158}
        .sp-tag.ext{background:rgba(56,189,248,.2);color:#38bdf8}
        .sp-tag.max{background:rgba(245,158,11,.22);color:#f59e0b}
        .sp-rec{display:flex;flex-direction:column}
        .sp-rrow{display:flex;align-items:center;justify-content:space-between;gap:12px;flex-wrap:wrap;padding:10px 0;border-top:1px solid var(--line);font-size:13.5px}
        .sp-rrow:first-child{border-top:none;padding-top:2px}
        .sp-rchips{display:flex;flex-wrap:wrap;gap:6px}
        .sp-rchip{display:inline-flex;align-items:center;gap:6px;padding:3px 9px 3px 5px;border-radius:8px;background:rgba(255,255,255,.05);border:1px solid var(--line);font-size:12px;font-weight:600;color:#ffffff}
        .sp-rchip img{width:22px;height:22px;object-fit:contain}
        /* Best Route Tab */
        #routePane{display:flex;flex-direction:column;gap:12px}
        .rt-top{display:flex;align-items:center;justify-content:space-between;gap:10px;flex-wrap:wrap}
        .rt-top select{flex:1;min-width:240px;max-width:440px;height:38px;min-height:38px;background:var(--card);border:1px solid var(--line-strong);border-radius:10px;color:#ffffff;font:600 13.5px var(--font);padding:0 12px}
        .rt-acts{display:flex;gap:8px;align-items:center;flex-wrap:wrap}
        .rt-stats{display:grid;grid-template-columns:repeat(auto-fit,minmax(130px,1fr));gap:10px}
        .rt-stat{background:var(--card);border:1px solid var(--line);border-radius:12px;padding:12px 14px}
        .rt-stat .v{font-size:22px;font-weight:700;color:#ffffff;font-variant-numeric:tabular-nums;letter-spacing:-.02em}
        .rt-stat .l{font-size:11.5px;color:var(--cmuted);margin-top:2px;font-weight:500}
        .rt-wrap{position:relative;background:var(--card);border:1px solid rgba(245,158,11,.28);border-radius:16px;padding:14px;display:flex;flex-direction:column;align-items:center;gap:10px}
        .rt-stage{position:relative;width:100%;max-width:680px;aspect-ratio:1/1;border-radius:12px;overflow:hidden;background:#0a2540;border:1px solid var(--line)}
        #rtCanvas{width:100%;height:100%;display:block;cursor:grab;touch-action:none}
        #rtCanvas.drag{cursor:grabbing}
        .rt-zctl{position:absolute;right:12px;bottom:12px;z-index:4;display:inline-flex;align-items:center;gap:2px;padding:3px;border-radius:10px;background:rgba(11,11,14,.86);border:1px solid var(--line-strong);backdrop-filter:blur(8px)}
        .rt-zctl button{width:28px;height:28px;border:none;border-radius:7px;background:transparent;color:#ffffff;font:700 15px var(--font);cursor:pointer;display:grid;place-items:center;transition:background .15s}
        .rt-zctl button:hover{background:rgba(255,255,255,.12)}
        .rt-zctl span{font-size:11px;font-weight:700;color:var(--cmuted);padding:0 6px;font-variant-numeric:tabular-nums;min-width:40px;text-align:center}
        .rt-tip{position:absolute;pointer-events:none;z-index:5;background:rgba(11,11,14,.92);border:1px solid rgba(245,158,11,.45);border-radius:8px;padding:6px 10px;font-size:12px;font-weight:600;color:#ffffff;white-space:nowrap;transform:translate(-50%,-120%);box-shadow:0 8px 20px rgba(0,0,0,.5)}
        .rt-leg{display:flex;flex-wrap:wrap;gap:14px;align-items:center;justify-content:center;font-size:12px;color:var(--cmuted)}
        .rt-leg span{display:inline-flex;align-items:center;gap:6px}
        .rt-dot{width:9px;height:9px;border-radius:50%;display:inline-block}
        </style></head><body><div class="tbar"><div class="wctl">
          <button type="button" id="wmin"><svg width="10" height="10" viewBox="0 0 10 10" aria-hidden="true"><path d="M0 5.5h10" stroke="currentColor" stroke-width="1.1"/></svg></button>
          <button type="button" id="wmax"><svg width="10" height="10" viewBox="0 0 10 10" aria-hidden="true"><rect id="wmaxicon" x=".55" y=".55" width="8.9" height="8.9" fill="none" stroke="currentColor" stroke-width="1.1"/><path id="wresicon" d="M2.5 2.5V.55h6.95v6.95H7.5M.55 2.5h6.95v6.95H.55z" fill="none" stroke="currentColor" stroke-width="1.1" style="display:none"/></svg></button>
          <button type="button" id="wclose" class="x"><svg width="10" height="10" viewBox="0 0 10 10" aria-hidden="true"><path d="M.5.5l9 9M9.5.5l-9 9" stroke="currentColor" stroke-width="1.2"/></svg></button>
        </div></div><div id="scroll"><main><div id="app"><div class="card empty-state"><div class="spin" aria-hidden="true"></div><div><div class="es-title">Chest Counter</div><div class="es-body">Loading replays and match stats…</div></div></div></div><section id="activityPane" class="hidden"><h2 class="pirate"></h2><div class="card log"></div></section><section id="reportPane" class="hidden"><iframe id="rep" title="Report" scrolling="no"></iframe></section>
        <section id="statsPane" class="hidden">
          <div class="gs-top">
            <form id="gsSearchForm" class="gs-search" autocomplete="off">
              <select id="gsPlatform" aria-label="Platform"><option value="epic">Epic</option><option value="psn">PSN</option><option value="xbl">Xbox</option></select>
              <span class="gs-div" aria-hidden="true"></span>
              <input id="gsUsername" type="text" placeholder="Search Fortnite username…" required>
              <button type="submit" id="gsLookupBtn" class="gs-btn-gold">Lookup</button>
            </form>
            <div class="gs-acts">
              <button type="button" id="gsLiveBtn" class="gs-ghost live-on" title="Auto-refresh stats every 30s and after matches">Live Watch: On</button>
              <button type="button" id="gsKeyBtn" class="gs-ghost" title="Configure Fortnite-API.com Key"><span class="gs-dot" id="gsKeyDot"></span>API Key</button>
            </div>
          </div>
          <div class="gs-ctrls">
            <div class="gs-seg" role="group" aria-label="Time Window">
              <button type="button" class="gs-pill on" data-gswin="season">Season</button>
              <button type="button" class="gs-pill" data-gswin="lifetime">Lifetime</button>
            </div>
            <div class="gs-seg" role="group" aria-label="Game Mode">
              <button type="button" class="gs-pill on" data-gsmode="overall">Overall</button>
              <button type="button" class="gs-pill" data-gsmode="solo">Solo</button>
              <button type="button" class="gs-pill" data-gsmode="duo">Duo</button>
              <button type="button" class="gs-pill" data-gsmode="squad">Squad</button>
              <button type="button" class="gs-pill" data-gsmode="ltm">LTM</button>
            </div>
            <div class="gs-seg" role="group" aria-label="Input Type">
              <button type="button" class="gs-pill on" data-gsin="all">All</button>
              <button type="button" class="gs-pill" data-gsin="keyboardMouse">KBM</button>
              <button type="button" class="gs-pill" data-gsin="gamepad">Pad</button>
            </div>
          </div>
          <div id="gsBanner" class="gs-banner hidden" role="alert"></div>
          <section id="gsCard" class="gs-card">
            <div class="gs-left">
              <div>
                <div class="gs-meta">
                  <span id="gsSeasonBadge" class="gs-meta-gold">SEASON STATS</span>
                  <span class="gs-meta-dot">•</span>
                  <span id="gsModeBadge" class="gs-meta-sub">OVERALL • ALL INPUTS</span>
                  <span id="gsSrcBadge" class="gs-src demo">DEMO PREVIEW</span>
                </div>
                <div class="gs-id">
                  <h2 id="gsPlayerName" class="gs-name">PLAYER</h2>
                  <div class="gs-lvl">
                    <span class="gs-lvl-l">LVL</span>
                    <span id="gsLevel" class="gs-lvl-v">—</span>
                    <div class="gs-bp" title="Battle Pass Progress"><div id="gsBp" class="gs-bp-f" style="width:0%"></div></div>
                  </div>
                </div>
              </div>
              <div class="gs-rows">
                <div class="gs-row">
                  <div class="gs-rico" aria-hidden="true"><svg viewBox="0 0 24 24" fill="currentColor"><path d="M19 5h-2V3H7v2H5c-1.1 0-2 .9-2 2v1c0 2.55 1.92 4.63 4.39 4.94.63 1.5 1.98 2.63 3.61 2.96V19H7v2h10v-2h-4v-3.1c1.63-.33 2.98-1.46 3.61-2.96C19.08 12.63 21 10.55 21 8V7c0-1.1-.9-2-2-2zM5 8V7h2v3.82C5.84 10.4 5 9.3 5 8zm14 0c0 1.3-.84 2.4-2 2.82V7h2v1z"/></svg></div>
                  <div class="gs-cells c3">
                    <div class="gs-cell"><div id="gsWins" class="gs-val">0</div><div class="gs-lbl">WINS</div></div>
                    <div class="gs-cell"><div id="gsWinRate" class="gs-val">0.0%</div><div class="gs-lbl">WIN RATE</div></div>
                    <div class="gs-cell"><div id="gsMatches" class="gs-val">0</div><div class="gs-lbl">MATCHES</div></div>
                  </div>
                </div>
                <div class="gs-row">
                  <div class="gs-rico" aria-hidden="true"><svg viewBox="0 0 24 24" fill="currentColor"><path d="M12 2a1 1 0 0 1 1 1v2.07A7.002 7.002 0 0 1 18.93 11H21a1 1 0 1 1 0 2h-2.07A7.002 7.002 0 0 1 13 18.93V21a1 1 0 1 1-2 0v-2.07A7.002 7.002 0 0 1 5.07 13H3a1 1 0 1 1 0-2h2.07A7.002 7.002 0 0 1 11 5.07V3a1 1 0 0 1 1-1zm0 5a5 5 0 1 0 0 10 5 5 0 0 0 0-10zm0 3a2 2 0 1 1 0 4 2 2 0 0 1 0-4z"/></svg></div>
                  <div class="gs-cells c3">
                    <div class="gs-cell"><div id="gsKD" class="gs-val">0.00</div><div class="gs-lbl">K/D</div></div>
                    <div class="gs-cell"><div id="gsKPM" class="gs-val">0.00</div><div class="gs-lbl">KILLS / MATCH</div></div>
                    <div class="gs-cell"><div id="gsKills" class="gs-val">0</div><div class="gs-lbl">KILLS</div></div>
                  </div>
                </div>
                <div class="gs-row">
                  <div class="gs-rico" aria-hidden="true"><svg viewBox="0 0 24 24" fill="currentColor"><path d="M15 1H9v2h6V1zm-4 13h2V8h-2v6zm8.03-6.61 1.42-1.42c-.43-.51-.9-.99-1.41-1.41l-1.42 1.42A8.962 8.962 0 0 0 12 4c-4.97 0-9 4.03-9 9s4.03 9 9 9 9-4.03 9-9c0-2.12-.74-4.07-1.97-5.61zM12 20c-3.87 0-7-3.13-7-7s3.13-7 7-7 7 3.13 7 7-3.13 7-7 7z"/></svg></div>
                  <div class="gs-cells c2">
                    <div class="gs-cell"><div id="gsPlayTime" class="gs-val">0H 0M</div><div class="gs-lbl">PLAY TIME</div></div>
                    <div class="gs-cell"><div id="gsAvgTime" class="gs-val">0M 0S</div><div class="gs-lbl">AVG. MATCH</div></div>
                  </div>
                </div>
              </div>
              <div class="gs-extra">
                <div class="gs-ex"><span class="gs-ex-l">TOP 3 / 5 / 10</span><span id="gsTop" class="gs-ex-v">0 / 0 / 0</span></div>
                <div class="gs-ex"><span class="gs-ex-l">OUTLIVED</span><span id="gsOutlived" class="gs-ex-v">0</span></div>
                <div class="gs-ex"><span class="gs-ex-l">SCORE</span><span id="gsScore" class="gs-ex-v">0</span></div>
                <div class="gs-ex"><span class="gs-ex-l">KILLS / MIN</span><span id="gsKMin" class="gs-ex-v">0.00</span></div>
              </div>
            </div>
            <div class="gs-right">
              <div class="gs-spot" aria-hidden="true"></div>
              <img id="gsSkinImg" class="gs-skin" src="" alt="Featured Outfit">
              <div id="gsSkinCap" class="gs-cap">MIDAS</div>
            </div>
          </section>
          <div class="gs-skinbar">
            <div class="gs-presets">
              <span class="gs-sbl">Outfit</span>
              <button type="button" class="gs-chip on" data-gsskin="Midas">Midas</button>
              <button type="button" class="gs-chip" data-gsskin="Marigold">Marigold</button>
              <button type="button" class="gs-chip" data-gsskin="Peely">Peely</button>
              <button type="button" class="gs-chip" data-gsskin="Aura">Aura</button>
              <button type="button" class="gs-chip" data-gsskin="Renegade Raider">Renegade Raider</button>
            </div>
            <form id="gsSkinForm" class="gs-sform">
              <input id="gsSkinInput" type="text" placeholder="Custom outfit…">
              <button type="submit" class="gs-ghost">Set</button>
            </form>
          </div>
        </section>
        <section id="routePane" class="hidden">
          <div class="rt-top">
            <select id="rtSelect" aria-label="Select match"></select>
            <div class="rt-acts">
              <div class="gs-seg">
                <button type="button" class="gs-pill on" id="rtZoomIsland">Island</button>
                <button type="button" class="gs-pill" id="rtZoomFit">Zoom Route</button>
              </div>
              <button type="button" class="gs-ghost live-on" id="rtPoisBtn">POIs: On</button>
              <button type="button" class="gs-ghost live-on" id="rtChestsBtn">Chests: On</button>
              <button type="button" class="gs-btn-gold" id="rtPlayBtn">▶ Replay</button>
              <button type="button" class="gs-ghost" id="rtGifBtn" title="Export animated GIF of this route to your Downloads folder">Export GIF</button>
            </div>
          </div>
          <div id="rtStats" class="rt-stats"></div>
          <div id="rtEmpty" class="card hidden"><span class="note">No route data recorded yet — existing replays are being re-scanned automatically, or play a match with Replays enabled.</span></div>
          <div id="rtWrap" class="rt-wrap">
            <div class="rt-stage">
              <canvas id="rtCanvas" width="900" height="900"></canvas>
              <div id="rtTip" class="rt-tip hidden"></div>
              <div class="rt-zctl">
                <button type="button" id="rtZoomOut" title="Zoom out">−</button>
                <span id="rtZoomLbl">1.0×</span>
                <button type="button" id="rtZoomIn" title="Zoom in">+</button>
              </div>
            </div>
            <div class="rt-leg">
              <span><i class="rt-dot" style="background:#30d158"></i>Landing</span>
              <span><i class="rt-dot" style="background:#f59e0b"></i>Chest opened</span>
              <span><i class="rt-dot" style="background:#ff453a"></i>Match end</span>
              <span><i style="display:inline-block;width:16px;height:3px;border-radius:2px;background:#f59e0b"></i>Route path</span>
            </div>
          </div>
        </section>
        <div id="foot"></div></main></div>
        <dialog id="dlg" aria-labelledby="dlgT"><div class="dhead"><h2 class="pirate" id="dlgT"></h2><button type="button" class="dclose" id="dlgX" aria-label="Close"><svg class="ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M18 6 6 18"/><path d="m6 6 12 12"/></svg></button></div><div id="dlgB"></div></dialog>
        <script>
        const TOKEN='__TOKEN__';let S=null,tab=(()=>{const h=location.hash.slice(1);if(['home','route','sprites','stats','report','overlay','settings','help'].includes(h))return h;try{return localStorage.getItem('cc-gui-tab')||'home'}catch(e){return 'home'}})(),estimate=null,stopped=false,copied=false,spFilter='all',spVar='all';
        const $=s=>document.querySelector(s);
        // In the counter's own window (?w=1): the title bar of the page, its buttons sent to the window.
        const host=window.chrome&&window.chrome.webview;let maximized=false;
        if(host&&new URLSearchParams(location.search).get('w')==='1')document.body.classList.add('windowed');
        const scroller=()=>document.body.classList.contains('windowed')?document.getElementById('scroll'):document.scrollingElement;
        function winBtn(a){if(host)host.postMessage({win:a})}
        function winTitles(){if(!S)return;const T=S.txt;$('#wmin').title=T.wMin;$('#wmin').setAttribute('aria-label',T.wMin);const m=maximized?T.wRestore:T.wMax;$('#wmax').title=m;$('#wmax').setAttribute('aria-label',m);$('#wclose').title=T.wClose;$('#wclose').setAttribute('aria-label',T.wClose)}
        if(host)host.addEventListener('message',e=>{const d=e.data;if(d&&typeof d.maximized==='boolean'){maximized=d.maximized;$('#wmaxicon').style.display=maximized?'none':'';$('#wresicon').style.display=maximized?'':'none';winTitles()}});
        const esc=s=>String(s??'').replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
        const fmt=(s,...a)=>a.reduce((r,v,i)=>r.replace('{'+i+'}',v),s);
        async function api(action,value){const r=await fetch('/api/action',{method:'POST',headers:{'X-Token':TOKEN,'Content-Type':'application/json'},body:JSON.stringify({action,value})});const j=await r.json();await refresh(true);return j.result}
        async function refresh(force){if(stopped)return;try{const r=await fetch('/api/state',{headers:{'X-Token':TOKEN}});S=await r.json();render(force)}catch(e){if(S){$('#app').innerHTML='<div class="card">'+esc(S.txt.offline)+'</div>';appHtml=null;$('#activityPane').classList.add('hidden')}}}
        function busy(){const a=document.activeElement;return a&&a.closest&&a.closest('#view')&&/INPUT|SELECT|TEXTAREA/.test(a.tagName)}
        function busyHeader(){const a=document.activeElement;return a&&a.closest&&a.closest('#hdr')&&a.tagName==='SELECT'}
        function sw(id,on,act){return `<label class="switch"><input type="checkbox" id="${id}" ${on?'checked':''} onchange="api('${act}',this.checked)"><span></span></label>`}
        function row(title,help,control){return `<div class="row"><div><div class="t">${title}</div>${help?`<div class="h">${help}</div>`:''}</div><div>${control}</div></div>`}
        function sel(act,value,opts){return `<select onchange="api('${act}',this.value)">${opts.map(o=>`<option value="${esc(o[0])}" ${String(o[0])===String(value)?'selected':''}>${esc(o[1])}</option>`).join('')}</select>`}
        function setTab(t){const changed=tab!==t;tab=t;try{localStorage.setItem('cc-gui-tab',t)}catch(e){}render(true);if(changed){['#view','#activityPane','#reportPane','#statsPane','#routePane'].forEach(sel=>{const el=$(sel);if(el){el.classList.remove('tab-anim');void el.offsetWidth;el.classList.add('tab-anim')}})}}
        function render(force){
          const T=S.txt;document.documentElement.lang=S.lang;
          const head=`<header><div class="brand"><img src="__CHESTICON__" width="46" height="46" alt="" style="object-fit:contain"><div><h1 class="pirate">${esc(S.appName)}</h1><div class="who">${S.profiles.length>1?`<select aria-label="${esc(T.profile)}" title="${esc(T.profile)}" onchange="api('profile',this.value)">${S.profiles.map(p=>`<option value="${esc(p.key)}" ${p.key===S.profileKey?'selected':''}>${esc(p.name)} (${esc(p.matches)})</option>`).join('')}</select>`:`<span>${esc(S.player||'')}</span>`}<span>v${esc(S.version)}</span></div></div></div><span class="pill ${S.status.kind}"><i></i>${esc(S.status.text)}</span></header>
          <nav>${['home','route','sprites','stats','report','overlay','settings','help'].map(t=>`<button class="${t===tab?'on':''}" onclick="setTab('${t}')">${esc(T[t])}</button>`).join('')}</nav>`;
          if(!force&&busyHeader())return;
          if(!force&&busy()){$('#hdr').innerHTML=head;appHtml=null;return}
          // A click in progress (button pressed, not released yet): rebuilding now would replace its button and the click
          // would be lost. The next refresh, 2 s later, catches up.
          if(!force&&pressedAt&&Date.now()-pressedAt<3000)return;
          let v='';
          if(tab==='home')v=home(T);else if(tab==='sprites')v=spritesView(T);else if(tab==='overlay')v=overlay(T);else if(tab==='settings')v=settings(T);else if(tab==='help')v=help(T);
          // Rebuilt only when something changed: the buttons stay the same elements between two refreshes.
          const app=`<div id="hdr">${head}</div>${notices(T)}<div id="view">${v}</div>`;
          if(force||app!==appHtml){$('#app').innerHTML=app;appHtml=app}
          // In the counter's own window the close button asks and stops it: no "quit" button needed.
          const foot=S.settings.windowed?`<footer><span>${esc(T.footerWindow)}</span></footer>`:`<footer><span>${esc(T.footer)}</span><button class="b danger" onclick="quit()">${esc(T.quit)}</button></footer>`;
          if(force||foot!==footHtml){$('#foot').innerHTML=foot;footHtml=foot}
          showActivity(T);showReport();showStats();showRoute();winTitles();
        }
        // pressedAt: when the mouse button went down (0 once released). Forgotten after 3 s in case the release was
        // missed (outside the window), so the page never stops refreshing.
        let appHtml=null,footHtml=null,pressedAt=0;
        document.addEventListener('pointerdown',()=>{pressedAt=Date.now()},true);
        ['pointerup','pointercancel'].forEach(t=>document.addEventListener(t,()=>{pressedAt=0},true));
        let activityHtml=null;
        function showActivity(T){
          const pane=$('#activityPane');
          pane.classList.toggle('hidden',tab!=='home');
          if(tab!=='home')return;
          pane.querySelector('h2').textContent=T.activity;
          // Keep the scrolling element mounted while new log entries arrive.
          const log=pane.querySelector('.log');
          const html=S.activity.length?S.activity.map(l=>l.map(s=>`<span class="${s.c}">${esc(s.t)}</span>`).join('')).join('\n'):esc(T.noActivity);
          if(activityHtml===html)return;
          const top=log.scrollTop;
          log.innerHTML=html;
          log.scrollTop=top;
          activityHtml=html;
        }
        // The report, in the window: loaded once, then again only when the counter rewrote it (new match, setting...).
        let repStamp=null,repObs=null;
        // Height of the report's content (not of its frame: it shrinks again when a list is folded).
        function fitReport(){const f=$('#rep');try{const m=f.contentDocument.querySelector('main');f.style.height=Math.ceil(m.getBoundingClientRect().height)+'px'}catch(e){}}
        function showReport(){
          const pane=$('#reportPane'),f=$('#rep');pane.classList.toggle('hidden',tab!=='report');
          if(tab!=='report'||S.reportStamp===repStamp)return;
          const y=scroller().scrollTop;repStamp=S.reportStamp;
          f.onload=()=>{fitReport();scroller().scrollTop=y;try{if(repObs)repObs.disconnect();repObs=new ResizeObserver(fitReport);repObs.observe(f.contentDocument.querySelector('main'))}catch(e){}};
          f.src='/report?t='+TOKEN+'&v='+encodeURIComponent(repStamp);
        }
        // Messages of content.json (a known problem, a fix to install...): above every tab.
        const starIco=`<svg class="ico ico-lg" style="color:var(--brand)" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>`;
        const alertIco=`<svg class="ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>`;
        const infoIco=`<svg class="ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><path d="M12 16v-4"/><path d="M12 8h.01"/></svg>`;
        const xIco=`<svg class="ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M18 6 6 18"/><path d="m6 6 12 12"/></svg>`;
        const bellIco=`<svg class="ico" style="color:var(--brand)" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M6 8a6 6 0 0 1 12 0c0 7 3 9 3 9H3s3-2 3-9"/><path d="M10.3 21a1.94 1.94 0 0 0 3.4 0"/></svg>`;
        const zapIco=`<svg class="ico" style="color:var(--brand)" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"/></svg>`;
        function notices(T){
          let out='';
          if(S.treasureAlert){const a=S.treasureAlert;out+=`<div class="card notice th-alert" role="alert"><span class="ni" aria-hidden="true">${bellIco}</span><div class="nb"><div class="nt">${esc(a.title)}</div><div class="nx">${esc(a.text)}</div></div><button class="nclose" title="${esc(T.close)}" aria-label="${esc(T.close)}" data-id="${esc(a.id)}" onclick="api('dismissTreasureAlert',this.dataset.id)">${xIco}</button></div>`;}
          out+=(S.notices||[]).map(n=>`<div class="card notice ${n.level==='warning'?'warning':''}" role="${n.level==='warning'?'alert':'status'}"><span class="ni" aria-hidden="true">${n.level==='warning'?alertIco:infoIco}</span><div class="nb">${n.title?`<div class="nt">${esc(n.title)}</div>`:''}<div class="nx">${esc(n.text)}</div>${n.link?`<div class="actions"><button class="b ${n.level==='warning'?'main':''}" data-link="${esc(n.link)}" onclick="window.open(this.dataset.link,'_blank','noopener,noreferrer')">${esc(T.moreInfo)}</button></div>`:''}</div>${n.level==='info'?`<button class="nclose" title="${esc(T.close)}" aria-label="${esc(T.close)}" data-id="${esc(n.id)}" onclick="api('dismissNotice',this.dataset.id)">${xIco}</button>`:''}</div>`).join('');
          return out;
        }
        function fmtCd(ms){if(ms<=0)return '0m';const s=Math.floor(ms/1000),d=Math.floor(s/86400),h=Math.floor((s%86400)/3600),m=Math.floor((s%3600)/60),sec=s%60;if(d>0)return `${d}d ${h}h ${m}m`;if(h>0)return `${h}h ${m}m`;return `${m}m ${sec}s`;}
        function goldenWeekCard(T){
          const gw=S.goldenWeek;if(!gw)return '';
          const now=Date.now(),s0=Date.parse(gw.start),e0=Date.parse(gw.end);
          if(now>=e0)return '';
          const live=now>=s0&&now<e0;
          const cdText=live?`LIVE · Ends in ${fmtCd(e0-now)}`:`Starts in ${fmtCd(s0-now)}`;
          const winRows=(gw.windows||[]).map((w,i)=>{
            const ws=Date.parse(w.start),we=Date.parse(w.end),isLive=now>=ws&&now<we;
            const pill=isLive?`<span class="th-pill live">${esc(T.thLive)}</span>`:(w.isToday?`<span class="th-pill">${esc(T.thToday)}</span>`:'');
            return `<div class="th-row"><span class="th-when">Kickoff Boost #${i+1} · ${esc(w.day)}${pill}</span><span class="th-time">${esc(w.range)}</span></div>`;
          }).join('');
          const foot=S.localZoneName?`All times shown in your local time · ${esc(S.localZoneName)}`:'All times shown in your local time';
          return `<section class="card gw-card">
            <div class="th-head"><h2 class="th-title">${starIco}<span>Fortnite Golden Week — Final Hunt</span></h2><span class="th-cd ${live?'live':''}" id="gwCd" data-start="${esc(gw.start)}" data-end="${esc(gw.end)}">${cdText}</span></div>
            <p class="gw-sub">All past seasonal treasures return to Battle Royale chests at the same time for the final week of Treasure Hunters — your last chance to pull any missed Solid Gold Outfits, Gold Vehicles &amp; Decals, Pickaxes, Back Blings, Jam Tracks, and Loading Screens before Season End.</p>
            <div class="gw-tags">
              <span class="gw-tag">All Past Treasures Return</span>
              <span class="gw-tag">Gold Outfits ×7.5+</span>
              <span class="gw-tag">Gold Vehicles &amp; Decals ×10</span>
              <span class="gw-tag">Pirate Shop Token ×1.5–2</span>
            </div>
            <div class="gw-grid">
              <div class="gw-box"><span class="gl">Kickoff &amp; Treasure Hours</span><span class="gv">${esc(gw.startLocal)}</span></div>
              <div class="gw-box"><span class="gl">Week 5 Final Hunt Rotation</span><span class="gv">${esc(gw.weekStartLocal)}</span></div>
              <div class="gw-box"><span class="gl">Event &amp; Season Ends</span><span class="gv">${esc(gw.endLocal)}</span></div>
            </div>
            ${winRows?`<div class="th-list">${winRows}</div>`:''}
            <div class="th-foot">${foot}</div>
          </section>`;
        }
        function treasureHoursCard(T){
          const list=S.treasureHours||[];if(!list.length)return '';
          const now=Date.now(),first=list[0],s0=Date.parse(first.start),e0=Date.parse(first.end),live=now>=s0&&now<e0;
          const loc=S.pcLocale||undefined;
          const dayFmt=new Intl.DateTimeFormat(loc,{weekday:'short',month:'short',day:'numeric'});
          const timeFmt=new Intl.DateTimeFormat(loc,{hour:'numeric',minute:'2-digit'});
          const todayKey=new Date(now).toDateString();
          const boosts=(first.boosts||[]).map(b=>`<span class="th-boost">${esc(b.name)} <b>×${esc(b.mult)}</b></span>`).join('');
          const rows=list.map(x=>{
            const sd=new Date(x.start),ed=new Date(x.end),isLive=now>=sd.getTime()&&now<ed.getTime(),isToday=typeof x.isToday==='boolean'?x.isToday:sd.toDateString()===todayKey;
            const pill=isLive?`<span class="th-pill live">${esc(T.thLive)}</span>`:(isToday?`<span class="th-pill">${esc(T.thToday)}</span>`:'');
            const dayText=x.day||dayFmt.format(sd);
            const timeText=x.range||`${timeFmt.format(sd)} – ${timeFmt.format(ed)}`;
            return `<div class="th-row"><span class="th-when">${esc(dayText)}${pill}</span><span class="th-time">${esc(timeText)}</span></div>`;
          }).join('');
          const cdText=live?`${esc(T.thEnds)} ${fmtCd(e0-now)}`:`${esc(T.thNext)} ${fmtCd(s0-now)}`;
          const foot=S.localZoneName?`${esc(T.thLocal)} · ${esc(S.localZoneName)}`:esc(T.thLocal);
          return `<section class="card th-card"><div class="th-head"><h2 class="th-title">${zapIco}<span>${esc(T.thTitle)}</span></h2><span class="th-cd ${live?'live':''}" id="thCd" data-start="${esc(first.start)}" data-end="${esc(first.end)}">${cdText}</span></div>${boosts?`<div class="th-boosts">${boosts}</div>`:''}<div class="th-list">${rows}</div><div class="th-foot">${foot}</div></section>`;
        }
        setInterval(()=>{if(document.hidden||!S)return;const T=S.txt,now=Date.now();const el=$('#thCd');if(el){const s=Date.parse(el.dataset.start),e=Date.parse(el.dataset.end),live=now>=s&&now<e;el.classList.toggle('live',live);el.textContent=live?`${T.thEnds} ${fmtCd(e-now)}`:`${T.thNext} ${fmtCd(s-now)}`;}const gw=$('#gwCd');if(gw){const s=Date.parse(gw.dataset.start),e=Date.parse(gw.dataset.end),live=now>=s&&now<e;gw.classList.toggle('live',live);gw.textContent=live?`LIVE · Ends in ${fmtCd(e-now)}`:`Starts in ${fmtCd(s-now)}`;}} ,1000);
        function home(T){
          let h='';
          if(S.autoClose)h+=`<div class="card notice warning"><b>${esc(S.status.text)}</b><br>${esc(T.autoCloseText)}<div class="actions" style="margin-top:10px"><button class="b main" onclick="api('keepOpen')">${esc(T.keepOpen)}</button></div></div>`;
          if(S.firstRun){const s=S.settings;h+=`<div class="card welcome"><h2 class="pirate">${esc(T.welcome)}</h2><p style="margin-top:0;color:#d1d1d6">${esc(T.welcomeText)}</p>
            <div class="t" style="font-weight:600;margin-top:14px;color:#ffffff">${esc(T.welcomeQuick)}</div>
            ${row(esc(T.inGame),esc(T.inGameHelp),sw('wov',S.overlay.show,'showOverlay'))}
            ${s.windowed?row(esc(T.tray),esc(T.trayHelp),sw('wtr',s.minimizeToTray,'minimizeToTray')):''}
            <div class="actions" style="margin-top:12px"><button class="b main" onclick="api('dismissWelcome')">${esc(T.welcomeDone)}</button></div></div>`}
          if(S.progress)h+=`<div class="card"><b>${esc(T.analyzing)} ${S.progress.done} / ${S.progress.total}</b><div class="bar"><i style="width:${Math.round(100*S.progress.done/Math.max(1,S.progress.total))}%"></i></div></div>`;
          if(S.whatsNew&&S.whatsNew.length)h+=`<div class="card"><h2 class="pirate">${esc(fmt(T.whatsNew,S.version))}</h2><ul>${S.whatsNew.map(n=>`<li>${esc(n)}</li>`).join('')}</ul><button class="b main" onclick="api('dismissWhatsNew')">${esc(T.ok)}</button></div>`;
          if(S.update)h+=`<div class="card"><h2 class="pirate">${esc(fmt(T.newVersion,S.update.version))}</h2><ul>${S.update.notes.map(n=>`<li>${esc(n)}</li>`).join('')}</ul><button class="b main" onclick="api('installUpdate')">${esc(T.install)}</button></div>`;
          h+=`<section class="tiles"><div class="tile hero"><button type="button" class="editbtn" onclick="openManual()" title="${esc(T.editChests)}" aria-label="${esc(T.editChests)}">__PENCIL__</button><div class="l">${esc(T.total)}</div><div class="v num">${esc(S.total)}</div><div class="l">${esc(T.totalSub)}${S.manual?'<br>'+esc(fmt(T.byHand,S.manualText.replace('+',''))):''}</div></div>
            <div class="tile"><div class="v num">${esc(S.session)}</div><div class="l">${esc(T.session)} · ${esc(S.sessionMatches)} ${esc(T.matches)}</div></div>
            <div class="tile"><div class="v num">${esc(S.last?S.last.chests:'—')}</div><div class="l">${esc(T.last)}${S.last?'<br>'+esc(S.last.when)+' · '+esc(S.last.mode):''}</div></div>
            <div class="tile"><div class="v num">${esc(S.avgPerDay||'0')}</div><div class="l">${esc(T.perDay)}${S.spanDays>0?'<br>over '+esc(S.spanDays)+(S.spanDays===1?' day':' days'):''}</div></div>
            ${S.avgPerWeek?`<div class="tile"><div class="v num">${esc(S.avgPerWeek)}</div><div class="l">${esc(T.perWeek)}<br>over ${(S.spanDays/7).toFixed(1)} weeks</div></div>`:''}
            ${S.squadSync&&S.squadSync.active?`<div class="tile" style="border-color:rgba(48,209,88,.35)"><div class="v num">${esc(S.squadSync.teamChests)}${S.squadSync.live?` <span class="th-pill live" style="margin-left:4px">${esc(T.thLive)}</span>`:''}</div><div class="l">Squad Sync chests · ${esc(S.squadSync.matches)} ${esc(T.matches)}<br>Squad total: ${esc(S.squadSync.totalChests)}${(S.squadSync.live?S.squadSync.livePeers:S.squadSync.recentPeers).length?'<br>'+((S.squadSync.live?S.squadSync.livePeers:S.squadSync.recentPeers).map(p=>esc(p.name)+': '+esc(p.chests)).join(' · ')):''}</div></div>`:''}
            <div class="tile"><div class="v num">${starIco}<span>${esc(S.treasures)}</span></div><div class="l">${esc(T.treasures)} · ${esc(S.matches)} ${esc(T.matches)} · ${esc(fmt(T.perMatch,S.average))}</div></div></section>`;
          if(S.matches==='0'&&!S.firstRun)h+=`<div class="card empty-state"><div class="es-body"><div class="es-title">${esc(T.total)}</div>${esc(T.emptyMatches)}</div></div>`;
          h+=`<div class="actions"><button class="b main" onclick="setTab('report')">${esc(T.openReport)}</button>`;
          if(S.overlay.running)h+=`<button class="b" onclick="api('toggleOverlayVisible')">${esc(S.overlay.visible?T.hideOverlay:T.showOverlay)}</button><button class="b" onclick="api('toggleOverlayLock')">${esc(S.overlay.locked?T.unlock:T.lock)}</button>`;
          if(!S.update&&S.updatesOn)h+=`<button class="b" onclick="api('checkUpdate')">${esc(T.checkUpdate)}</button>`;
          h+='</div>';
          h+=goldenWeekCard(T);
          h+=treasureHoursCard(T);
          return h;
        }
        function setSpFilter(f){spFilter=f;render(true)}
        function setSpVar(v){spVar=v;render(true)}
        function spritesView(T){
          const sp=S.sprites;if(!sp)return '';
          let h=`<section class="tiles">
            <div class="tile hero"><div class="l">Sprite Collection</div><div class="v num">${esc(sp.ownedCount)} <span style="font-size:20px;color:var(--cmuted);font-weight:600">/ ${esc(sp.totalVariants)}</span></div><div class="l">Auto-tracked from extractions & wins, or click any variant below to tick manually</div></div>
            <div class="tile"><div class="v num">${esc(sp.extractedCount)}</div><div class="l">Extracted variants</div></div>
            <div class="tile"><div class="v num">${esc(sp.wonCount)}</div><div class="l">Secured from wins</div></div>
            <div class="tile"><div class="v num">${esc(sp.maxedCount)}</div><div class="l">Maxed to Lv 5</div></div>
            <div class="tile"><div class="v num">${esc(sp.seenCount)}</div><div class="l">Carried in replays</div></div>
          </section>`;
          h+=`<div class="sp-bar">
            <div class="gs-seg">${[['all','All'],['owned','Owned'],['missing','Missing'],['secured','Extracted / Won']].map(([k,l])=>`<button type="button" class="gs-pill ${spFilter===k?'on':''}" onclick="setSpFilter('${k}')">${l}</button>`).join('')}</div>
            <div class="gs-seg">${[['all','All Variants'],['base','Base'],['fortune','Fortune'],['cursed','Cursed'],['gold','Gold']].map(([k,l])=>`<button type="button" class="gs-pill ${spVar===k?'on':''}" onclick="setSpVar('${k}')">${l}</button>`).join('')}</div>
          </div>`;
          const cards=[];
          for(const fam of sp.families){
            const vars=fam.variants.filter(v=>{
              if(spVar!=='all'&&v.variant!==spVar)return false;
              if(spFilter==='owned'&&!v.owned)return false;
              if(spFilter==='missing'&&v.owned)return false;
              if(spFilter==='secured'&&!(v.extracted>0||v.won>0))return false;
              return true;
            });
            if(!vars.length)continue;
            const chips=vars.map(v=>{
              const tags=[];
              if(v.won>0)tags.push(`<span class="sp-tag won">Won${v.won>1?' ×'+v.won:''}</span>`);
              if(v.extracted>0)tags.push(`<span class="sp-tag ext">Ext${v.extracted>1?' ×'+v.extracted:''}</span>`);
              if(v.maxed)tags.push(`<span class="sp-tag max">Lv5</span>`);
              if(!tags.length&&v.seen>0)tags.push(`<span class="sp-tag">×${v.seen}</span>`);
              const check=v.owned?'✓ ':'';
              const imgUrl=`/sprites/${encodeURIComponent(fam.id+'-'+v.variant+'.webp')}`;
              return `<button type="button" class="sp-var v-${esc(v.variant)} ${v.owned?'owned':''}" title="Click to toggle owned" onclick="api('toggleSprite','${esc(v.key)}')"><img src="${imgUrl}" alt="${esc(v.label+' '+fam.name)}" loading="lazy"><span>${check}${esc(v.label)}</span><span class="sp-tags">${tags.join('')}</span></button>`;
            }).join('');
            const baseUrl=`/sprites/${encodeURIComponent(fam.id+'-base.webp')}`;
            cards.push(`<div class="sp-card"><div class="sp-head"><img class="sp-thumb" src="${baseUrl}" alt="${esc(fam.name)}" loading="lazy"><div class="sp-hinfo"><div class="sp-htop"><span class="sp-name">${esc(fam.name)}</span><span class="sp-rar ${esc(fam.rarity.toLowerCase())}">${esc(fam.rarity)}</span></div><div class="sp-ab">${esc(fam.ability)}</div></div></div><div class="sp-vars ${vars.length===1?'single':''}">${chips}</div></div>`);
          }
          h+=cards.length?`<div class="sp-grid">${cards.join('')}</div>`:`<div class="card"><span class="note">No Sprites match this filter yet.</span></div>`;
          if(sp.recent&&sp.recent.length){
            const rows=sp.recent.map(r=>{
              const pl=r.placement?` · #${r.placement}`:'';
              const chips=r.items.map(i=>{
                const b=[];
                if(i.won)b.push('<span class="sp-tag won">Won</span>');
                if(i.extracted)b.push('<span class="sp-tag ext">Extracted</span>');
                if(i.maxed)b.push('<span class="sp-tag max">Lv5</span>');
                const iu=`/sprites/${encodeURIComponent((i.familyId||'')+'-'+(i.variant||'base')+'.webp')}`;
                return `<span class="sp-rchip"><img src="${iu}" alt="" onerror="this.style.display='none'">${esc(i.variantLabel)} ${esc(i.name)} ${b.join('')}</span>`;
              }).join('');
              return `<div class="sp-rrow"><span style="color:var(--cmuted)">${esc(r.when)} · ${esc(r.mode)}${esc(pl)}</span><div class="sp-rchips">${chips}</div></div>`;
            }).join('');
            h+=`<section><h2 class="pirate">Recent Replay Sprites</h2><div class="card sp-rec">${rows}</div></section>`;
          }
          return h;
        }
        function overlay(T){
          const o=S.overlay;
          let h=`<div class="card">${row(esc(T.inGame),esc(T.inGameHelp),sw('ov',o.show,'showOverlay'))}
            ${row(esc(T.style),'',sel('overlayStyle',o.style,[['hud',T.hud],['classic',T.classic]]))}
            ${o.show?row(esc(T.size),'',sel('overlaySize',o.size,o.sizes.map(x=>[x,x+' %'])))+row(esc(T.opacity),'',sel('overlayOpacity',o.opacity,o.opacities.map(x=>[x,x+' %']))):''}</div>
            <section><h2 class="pirate">${esc(T.obs)}</h2><div class="card"><span class="path">${esc(S.paths.overlay)}</span><p class="note" style="color:var(--cmuted)">${esc(T.obsSteps)}</p>
            <div class="actions"><button class="b main" onclick="copyObs()">${esc(copied?T.copied:T.copy)}</button><button class="b" onclick="window.open('/overlay?t='+TOKEN,'_blank')">${esc(T.preview)}</button></div></div></section>`;
          return h;
        }
        function settings(T){
          const s=S.settings;
          let h=`<div class="card">${row(esc(T.times),esc(T.timesHelp),sel('timeZone',s.timeZone,[['local',fmt(T.timesLocal,s.localZone)],['utc',T.timesUtc]]))}
            ${row(esc(T.clock),'',sel('clockFormat',s.clockFormat,[['auto',fmt(T.clockAuto,s.windowsClock==='24'?T.h24:T.h12)],['12',T.clock12],['24',T.clock24]]))}
            ${S.profiles.length>1?row(esc(T.profile),'',sel('profile',S.profileKey,S.profiles.map(p=>[p.key,p.name+' ('+p.matches+' · '+p.chests+')']))):''}
            ${s.windowed?row(esc(T.tray),esc(T.trayHelp),sw('tr',s.minimizeToTray,'minimizeToTray')):''}
            ${s.canStartWithFortnite?row(esc(T.withFortnite),esc(T.withFortniteHelp),sw('wf',s.startWithFortnite,'startWithFortnite')):''}
            ${row(esc(T.zapMode),esc(T.zapModeHelp),sw('zm',s.zapMode,'zapMode'))}
            ${row(esc(T.treasureAlerts),esc(T.treasureAlertsHelp),sw('tha',s.treasureHourAlerts,'treasureHourAlerts'))}
            ${row(esc(T.squadSync),esc(T.squadSyncHelp),sw('sqs',s.squadSync,'squadSync'))}
            ${row(esc(T.discordStatus),esc(T.discordStatusHelp),sw('drpc',s.discordPresence,'discordPresence'))}
            ${s.discordPresence?`<div class="row"><div style="flex:1"><div class="t">${esc(T.discordAppId)}</div><div class="actions" style="margin-top:6px"><input type="text" id="discordAppId" placeholder="1234567890123456789" value="${esc(s.discordAppId||'')}"><button class="b" onclick="api('discordAppId',$('#discordAppId').value)">${esc(T.saveId)}</button></div></div></div>`:''}
            ${row(esc(T.updates),'',sel('updateMode',s.updateMode,[['prompt',T.upPrompt],['auto',T.upAuto],['off',T.upOff]]))}
            <div class="row"><div style="flex:1"><div class="t">${esc(T.demos)}</div>${s.demosExists?'':`<div class="h">${esc(T.demosMissing)}</div>`}<div class="actions" style="margin-top:6px"><input type="text" id="demos" value="${esc(s.demos)}"><button class="b" onclick="api('demosFolder',$('#demos').value)">${esc(T.save)}</button></div></div></div></div>`;
          return h;
        }
        // Chests added by hand (and the estimate from the Career matches): p = 's' in the settings, 'd' in the edit box.
        // The box starts empty each time: the number typed is added to what was already added, and each addition can be
        // undone with its cross (issue #27).
        function manualForm(T,p){
          const hist=S.manualHistory||[];
          return `<p class="note" style="color:var(--cmuted);margin-top:0">${esc(T.manualHelp)}</p>
            <div class="mtotal">${esc(fmt(T.manualTotal,S.manualText))}</div>
            <div class="actions"><input type="number" id="${p}manual" step="1" placeholder="0" aria-label="${esc(T.manual)}" onkeydown="if(event.key==='Enter')addManual('${p}')"><button class="b main" onclick="addManual('${p}')">${esc(T.addBtn)}</button></div>
            ${S.manualWeek?`<label class="mweek"><input type="checkbox" id="${p}week"><b>${esc(T.thisWeek)}</b><span>${esc(fmt(T.thisWeekHelp,S.manualWeek))}</span></label>`:''}
            ${hist.length?`<ul class="mhist">${hist.map(e=>`<li><b class="num">${esc(e.chests)}</b><span>${esc([e.what,e.when].filter(Boolean).join(' · '))}</span><button type="button" class="mx" title="${esc(T.removeEntry)}" aria-label="${esc(T.removeEntry)}" data-id="${esc(e.id)}" onclick="removeManual('${p}',this.dataset.id)">×</button></li>`).join('')}</ul>`:''}
            <div class="t" style="margin-top:16px;font-weight:600">${esc(T.estimate)}</div><p class="note" style="color:var(--cmuted)">${esc(T.estimateHelp)}</p>
            <div class="actions"><input type="number" id="${p}career" min="0" placeholder="${esc(T.career)}" aria-label="${esc(T.career)}"><button class="b" onclick="doEstimate('${p}')">${esc(T.compute)}</button></div>
            <div class="estimate" id="${p}est">${estimate&&estimate.p===p?estimateHtml(T,p):''}</div>`;
        }
        function estimateHtml(T,p){if(estimate.error)return esc(estimate.error);return esc(estimate.text)+`<div class="actions" style="margin-top:8px"><button class="b main" onclick="addEstimate('${p}')">${esc(fmt(T.add,estimate.chestsText))}</button></div>`}
        async function doEstimate(p){const r=await api('estimate',parseInt($('#'+p+'career').value||'0',10));estimate=Object.assign({p},r);const e=$('#'+p+'est');if(e)e.innerHTML=estimateHtml(S.txt,p)}
        async function addManual(p){const i=$('#'+p+'manual'),w=$('#'+p+'week'),n=parseInt(i&&i.value||'0',10);if(!n){if(i)i.focus();return}await api('manualAdd',{chests:n,thisWeek:!!(w&&w.checked)});redrawManual(p)}
        async function addEstimate(p){if(!estimate||estimate.error)return;await api('manualEstimate',{chests:estimate.chests,career:estimate.career});redrawManual(p)}
        async function removeManual(p,id){await api('manualRemove',id);redrawManual(p)}
        // After a change the box stays open, with the new total and history, ready for the next number.
        function redrawManual(p){estimate=null;if(p!=='d'){render(true);return}$('#dlgB').innerHTML=manualForm(S.txt,'d');const i=$('#dmanual');if(i)i.focus()}
        // The edit box of "chests in total" (home tab, and the report's pencil through a message).
        function openManual(){const T=S.txt,d=$('#dlg');estimate=null;$('#dlgT').textContent=T.editChests;$('#dlgB').innerHTML=manualForm(T,'d');$('#dlgX').setAttribute('aria-label',T.close);if(!d.open)d.showModal();const i=$('#dmanual');if(i){i.focus();i.select()}}
        $('#dlgX').onclick=()=>$('#dlg').close();
        $('#wmin').onclick=()=>winBtn('min');$('#wmax').onclick=()=>winBtn('max');$('#wclose').onclick=()=>winBtn('close');
        // A click on the dimmed backdrop closes the box, only when it starts there too: selecting the number with the mouse
        // and releasing the button outside the box must not close it.
        let dlgDownOut=false;
        function dlgOutside(e){const r=$('#dlg').getBoundingClientRect();return e.target.id==='dlg'&&(e.clientX<r.left||e.clientX>r.right||e.clientY<r.top||e.clientY>r.bottom)}
        $('#dlg').addEventListener('pointerdown',e=>{dlgDownOut=dlgOutside(e)});
        $('#dlg').addEventListener('click',e=>{if(dlgDownOut&&dlgOutside(e))$('#dlg').close();dlgDownOut=false});
        window.addEventListener('message',e=>{if(e.origin===location.origin&&e.data&&e.data.cc==='editChests'&&S)openManual()});
        async function copyObs(){await api('copyObsPath');copied=true;render(true);setTimeout(()=>{copied=false;render(true)},2000)}
        function help(T){
          return `<div class="card"><p style="margin-top:0">${esc(T.helpText)}</p><div class="actions"><button class="b" onclick="api('openDataFolder')">${esc(T.dataFolder)}</button>${S.paths.errorLog?`<button class="b" onclick="api('openErrorLog')">${esc(T.errorLog)}</button>`:''}<button class="b" onclick="api('openIssues')">${esc(T.issue)}</button><button class="b" onclick="api('openRepo')">${esc(T.project)}</button></div></div>
          ${S.canUninstall?`<section><h2 class="pirate">${esc(T.uninstall)}</h2><div class="card"><span class="path">${esc(S.paths.data)}</span>
          <div class="actions"><button class="b danger" onclick="uninstall(false)">${esc(T.uninstall)} · ${esc(T.keepData)}</button><button class="b danger" onclick="uninstall(true)">${esc(T.uninstall)} · ${esc(T.deleteData)}</button></div></div></section>`:''}`;
        }
        function closedView(){stopped=true;$('#app').innerHTML='<div class="card">'+esc(S.txt.closed)+'</div>';$('#foot').innerHTML='';appHtml=footHtml=null;$('#activityPane').classList.add('hidden');$('#reportPane').classList.add('hidden');$('#statsPane').classList.add('hidden');$('#routePane').classList.add('hidden')}
        async function uninstall(all){if(!confirm(S.txt.uninstallConfirm))return;const err=await api('uninstall',all);if(err){alert(err);return}closedView()}
        async function quit(){if(!confirm(S.txt.quitConfirm))return;try{await api('quit')}catch(e){}closedView()}
        // --- Gold Stats Card ---
        const GS_DEMO={account:{name:'Player'},battlePass:{level:212,progress:68},stats:{all:{overall:{wins:77,winRate:20.3,matches:379,kd:5.29,killsPerMatch:4.21,kills:1597,minutesPlayed:3432,top3:112,top5:145,top10:210,playersOutlived:24810,score:148920,killsPerMin:0.46}}}};
        const gs={init:false,win:'season',mode:'overall',inp:'all',data:GS_DEMO,isDemo:true,live:true,timer:null,snap:null,lastMatchCount:null};
        const gsAnims=new WeakMap(),gsPrev=new WeakMap();
        function gsFmtInt(n){return Number(n||0).toLocaleString('en-US')}
        function gsFmtTime(m){const t=Math.max(0,Math.round(m||0));return `${Math.floor(t/60)}H ${t%60}M`}
        function gsAnim(el,target,fn,dur=650){
          const prev=gsPrev.get(el);if(prev===target){el.textContent=fn(target);return}
          const start=typeof prev==='number'&&!Number.isNaN(prev)?prev:0;gsPrev.set(el,target);
          if(gsAnims.has(el))cancelAnimationFrame(gsAnims.get(el));
          el.classList.remove('gs-roll');void el.offsetWidth;el.classList.add('gs-roll');
          const t0=performance.now();
          const step=now=>{const p=Math.min(1,(now-t0)/dur),e=1-Math.pow(1-p,3);el.textContent=fn(p<1?start+(target-start)*e:target);if(p<1)gsAnims.set(el,requestAnimationFrame(step));else gsAnims.delete(el)};
          gsAnims.set(el,requestAnimationFrame(step));
        }
        function gsBanner(msg,err){const b=$('#gsBanner');if(!msg){b.classList.add('hidden');return}b.textContent=msg;b.classList.toggle('err',!!err);b.classList.remove('hidden')}
        function gsRender(){
          const d=gs.data,b=(d&&d.stats&&(d.stats[gs.inp]||d.stats.all))||{},m=b[gs.mode];
          $('#gsPlayerName').textContent=(d&&d.account&&d.account.name)||$('#gsUsername').value.trim()||'PLAYER';
          const lv=d&&d.battlePass&&d.battlePass.level;
          if(typeof lv==='number')gsAnim($('#gsLevel'),lv,v=>Math.round(v).toString());else $('#gsLevel').textContent='—';
          $('#gsBp').style.width=`${Math.min(100,Math.max(0,(d&&d.battlePass&&d.battlePass.progress)||0))}%`;
          $('#gsSeasonBadge').textContent=gs.win==='season'?'SEASON STATS':'LIFETIME STATS';
          const il=gs.inp==='all'?'ALL INPUTS':gs.inp==='keyboardMouse'?'KBM':'CONTROLLER';
          $('#gsModeBadge').textContent=`${gs.mode.toUpperCase()} • ${il}`;
          $('#gsSrcBadge').textContent=gs.isDemo?'DEMO PREVIEW':'LIVE API';
          $('#gsSrcBadge').className=`gs-src ${gs.isDemo?'demo':'live'}`;
          if(!m){
            gsAnim($('#gsWins'),0,v=>gsFmtInt(Math.round(v)));gsAnim($('#gsWinRate'),0,v=>`${v.toFixed(1)}%`);gsAnim($('#gsMatches'),0,v=>gsFmtInt(Math.round(v)));
            gsAnim($('#gsKD'),0,v=>v.toFixed(2));gsAnim($('#gsKPM'),0,v=>v.toFixed(2));gsAnim($('#gsKills'),0,v=>gsFmtInt(Math.round(v)));
            gsAnim($('#gsPlayTime'),0,v=>gsFmtTime(v));$('#gsAvgTime').textContent='0M 0S';$('#gsTop').textContent='0 / 0 / 0';
            gsAnim($('#gsOutlived'),0,v=>gsFmtInt(Math.round(v)));gsAnim($('#gsScore'),0,v=>gsFmtInt(Math.round(v)));gsAnim($('#gsKMin'),0,v=>v.toFixed(2));
            return;
          }
          gsAnim($('#gsWins'),Number(m.wins||0),v=>gsFmtInt(Math.round(v)));
          gsAnim($('#gsWinRate'),Number(m.winRate||0),v=>`${v.toFixed(1)}%`);
          gsAnim($('#gsMatches'),Number(m.matches||0),v=>gsFmtInt(Math.round(v)));
          gsAnim($('#gsKD'),Number(m.kd||0),v=>v.toFixed(2));
          gsAnim($('#gsKPM'),Number(m.killsPerMatch||0),v=>v.toFixed(2));
          gsAnim($('#gsKills'),Number(m.kills||0),v=>gsFmtInt(Math.round(v)));
          const mins=Number(m.minutesPlayed||0),mc=Number(m.matches||0),avgS=mc>0?Math.round((mins*60)/mc):0;
          gsAnim($('#gsPlayTime'),mins,v=>gsFmtTime(v));
          gsAnim($('#gsAvgTime'),avgS,s=>{const r=Math.max(0,Math.round(s));return `${Math.floor(r/60)}M ${r%60}S`});
          const tA=m.top3??m.top5??0,tB=m.top6??m.top10??0,tC=m.top12??m.top25??0;
          $('#gsTop').textContent=`${gsFmtInt(tA)} / ${gsFmtInt(tB)} / ${gsFmtInt(tC)}`;
          gsAnim($('#gsOutlived'),Number(m.playersOutlived||0),v=>gsFmtInt(Math.round(v)));
          gsAnim($('#gsScore'),Number(m.score||0),v=>gsFmtInt(Math.round(v)));
          gsAnim($('#gsKMin'),Number(m.killsPerMin||0),v=>v.toFixed(2));
        }
        async function gsLookup(name,{silent=false}={}){
          name=(name||'').trim();if(!name)return;
          const btn=$('#gsLookupBtn');
          if(!silent){gsBanner('');btn.disabled=true;btn.textContent='Loading…'}
          const plat=$('#gsPlatform').value;
          try{
            const q=new URLSearchParams({name,accountType:plat,timeWindow:gs.win});
            const r=await fetch('/api/fn-stats?'+q.toString(),{headers:{'X-Token':TOKEN}});
            const j=await r.json();
            if(!r.ok){
              if(!silent){gsBanner(j.error||'Could not fetch player stats.',true);if(j.code==='NO_API_KEY'||j.code==='INVALID_API_KEY')openFnKeyModal()}
              return;
            }
            if(j&&j.data){
              const snap=JSON.stringify({bp:j.data.battlePass,all:j.data.stats&&j.data.stats.all&&j.data.stats.all.overall});
              const changed=gs.snap!==null&&gs.snap!==snap;gs.snap=snap;
              gs.data=j.data;gs.isDemo=false;gsRender();
              if(changed){const c=$('#gsCard');c.classList.remove('flash');void c.offsetWidth;c.classList.add('flash')}
              if(!silent)api('fnStatsPrefs',{player:name,platform:plat});
            }
          }catch(e){if(!silent)gsBanner('Network error while fetching stats.',true)}
          finally{if(!silent){btn.disabled=false;btn.textContent='Lookup'}}
        }
        async function gsLoadSkin(name,save){
          try{
            const r=await fetch('/api/fn-skin?name='+encodeURIComponent(name),{headers:{'X-Token':TOKEN}});
            if(!r.ok)return;const s=await r.json();
            if(s.image){$('#gsSkinImg').src=s.image;$('#gsSkinCap').textContent=(s.name||name).toUpperCase();if(save)api('fnStatsPrefs',{outfit:s.name||name})}
          }catch(e){}
        }
        function openFnKeyModal(){
          const d=$('#dlg');$('#dlgT').textContent='Fortnite API Key';
          $('#dlgB').innerHTML=`<p class="note" style="color:var(--cmuted);margin-top:0">Live player stats use <b>fortnite-api.com</b>. Get a free API key at <a href="https://dash.fortnite-api.com/" target="_blank" rel="noopener noreferrer" style="color:var(--brand)">dash.fortnite-api.com</a> and paste it below. Stored locally in your config only.</p>
            <div class="actions" style="margin-top:12px"><input type="text" id="fnKeyIn" placeholder="Paste your Fortnite-API.com key..." autocomplete="off"></div>
            <div class="actions" style="margin-top:14px"><button type="button" class="b main" id="fnKeySave">Save Key</button><button type="button" class="b danger" id="fnKeyClear">Clear</button></div>`;
          $('#fnKeySave').onclick=async()=>{const k=$('#fnKeyIn').value.trim();await api('fnApiKey',k);d.close();gsBanner('');const u=$('#gsUsername').value.trim();if(u&&k)gsLookup(u)};
          $('#fnKeyClear').onclick=async()=>{await api('fnApiKey','');d.close()};
          if(!d.open)d.showModal();const i=$('#fnKeyIn');if(i)i.focus();
        }
        function gsStartPoll(){
          if(gs.timer)clearInterval(gs.timer);
          gs.timer=setInterval(()=>{if(!document.hidden&&tab==='stats'&&gs.live&&!gs.isDemo){const u=$('#gsUsername').value.trim();if(u)gsLookup(u,{silent:true})}},30000);
        }
        function showStats(){
          const pane=$('#statsPane');pane.classList.toggle('hidden',tab!=='stats');
          if(!S||!S.fnStats)return;
          const dot=$('#gsKeyDot');if(dot)dot.classList.toggle('ok',!!S.fnStats.hasKey);
          if(gs.lastMatchCount!==null&&gs.lastMatchCount!==S.matches&&gs.live&&!gs.isDemo){
            const u=$('#gsUsername').value.trim();if(u)gsLookup(u,{silent:true});
          }
          gs.lastMatchCount=S.matches;
          if(tab!=='stats')return;
          if(!gs.init){
            gs.init=true;
            $('#gsUsername').value=S.fnStats.player||'';
            $('#gsPlatform').value=S.fnStats.platform||'epic';
            const out=S.fnStats.outfit||'Midas';
            document.querySelectorAll('.gs-chip').forEach(c=>c.classList.toggle('on',c.dataset.gsskin.toLowerCase()===out.toLowerCase()));
            gsRender();gsLoadSkin(out,false);gsStartPoll();
            $('#gsSearchForm').addEventListener('submit',e=>{e.preventDefault();gsLookup($('#gsUsername').value)});
            $('#gsKeyBtn').addEventListener('click',openFnKeyModal);
            $('#gsLiveBtn').addEventListener('click',()=>{gs.live=!gs.live;$('#gsLiveBtn').textContent=gs.live?'Live Watch: On':'Live Watch: Off';$('#gsLiveBtn').classList.toggle('live-on',gs.live)});
            document.querySelectorAll('[data-gswin]').forEach(b=>b.addEventListener('click',()=>{document.querySelectorAll('[data-gswin]').forEach(x=>x.classList.remove('on'));b.classList.add('on');gs.win=b.dataset.gswin;if(!gs.isDemo)gsLookup($('#gsUsername').value);else gsRender()}));
            document.querySelectorAll('[data-gsmode]').forEach(b=>b.addEventListener('click',()=>{document.querySelectorAll('[data-gsmode]').forEach(x=>x.classList.remove('on'));b.classList.add('on');gs.mode=b.dataset.gsmode;gsRender()}));
            document.querySelectorAll('[data-gsin]').forEach(b=>b.addEventListener('click',()=>{document.querySelectorAll('[data-gsin]').forEach(x=>x.classList.remove('on'));b.classList.add('on');gs.inp=b.dataset.gsin;gsRender()}));
            document.querySelectorAll('.gs-chip').forEach(c=>c.addEventListener('click',()=>{document.querySelectorAll('.gs-chip').forEach(x=>x.classList.remove('on'));c.classList.add('on');gsLoadSkin(c.dataset.gsskin,true)}));
            $('#gsSkinForm').addEventListener('submit',e=>{e.preventDefault();const v=$('#gsSkinInput').value.trim();if(v){document.querySelectorAll('.gs-chip').forEach(x=>x.classList.remove('on'));gsLoadSkin(v,true)}});
            if(S.fnStats.hasKey&&S.fnStats.player)gsLookup(S.fnStats.player);
          }
        }
        // --- Best Route Map ---
        const rt={init:false,zoom:'island',customVp:null,drag:null,pois:true,chests:true,selectedId:null,listSig:'',playing:false,animId:null,prog:1,mapPoi:null,mapBlank:null,hitPts:[]};
        function rtWorldToNorm(x,y){return [(x+150000)/310000,(y+155000)/310000]}
        function rtFmtSec(s){const m=Math.floor((s||0)/60),r=Math.floor((s||0)%60);return `${m}:${r<10?'0':''}${r}`}
        function rtCurMatch(){const list=(S&&S.bestRoutes)||[];if(!list.length)return null;return list.find(m=>m.id===rt.selectedId)||list[0]}
        function rtClampVp(u0,v0,span){span=Math.max(0.06,Math.min(0.92,span));return {u0:Math.max(0,Math.min(1-span,u0)),v0:Math.max(0,Math.min(1-span,v0)),span}}
        function rtBaseViewport(m){
          if(rt.zoom==='island')return {u0:0.18,v0:0.20,span:0.68};
          const pts=[];
          const r=m.route||[];for(let i=0;i+2<r.length;i+=3)pts.push(rtWorldToNorm(r[i+1],r[i+2]));
          for(const c of (m.chestPts||[]))pts.push(rtWorldToNorm(c.x,c.y));
          if(!pts.length)return {u0:0.18,v0:0.20,span:0.68};
          let minU=1,maxU=0,minV=1,maxV=0;
          for(const [u,v] of pts){if(u<minU)minU=u;if(u>maxU)maxU=u;if(v<minV)minV=v;if(v>maxV)maxV=v}
          const pad=0.05,span=Math.max(0.14,Math.max(maxU-minU,maxV-minV)+pad*2);
          const cu=(minU+maxU)/2,cv=(minV+maxV)/2;
          return rtClampVp(cu-span/2,cv-span/2,span);
        }
        function rtViewport(m){return rt.customVp||rtBaseViewport(m)}
        function rtUpdateZoomLbl(){const m=rtCurMatch(),el=$('#rtZoomLbl');if(!m||!el)return;const vp=rtViewport(m);el.textContent=(0.68/vp.span).toFixed(1)+'×'}
        function rtZoomBy(factor,fx,fy){
          const m=rtCurMatch();if(!m)return;
          const vp=rtViewport(m);
          const newSpan=Math.max(0.06,Math.min(0.92,vp.span*factor));
          const uAt=vp.u0+fx*vp.span,vAt=vp.v0+fy*vp.span;
          rt.customVp=rtClampVp(uAt-fx*newSpan,vAt-fy*newSpan,newSpan);
          rtUpdateZoomLbl();rtDraw();
        }
        function rtDrawOn(cv,m,prog,withHud){
          if(!cv||!m)return;
          const ctx=cv.getContext('2d'),W=cv.width,H=cv.height;
          ctx.clearRect(0,0,W,H);ctx.fillStyle='#0a2540';ctx.fillRect(0,0,W,H);
          const vp=rtViewport(m),toPx=(x,y)=>{const [u,v]=rtWorldToNorm(x,y);return [(u-vp.u0)/vp.span*W,(v-vp.v0)/vp.span*H]};
          const img=rt.pois?rt.mapPoi:rt.mapBlank;
          if(img&&img.complete&&img.naturalWidth>0){
            const iw=img.naturalWidth,ih=img.naturalHeight;
            ctx.drawImage(img,vp.u0*iw,vp.v0*ih,vp.span*iw,vp.span*ih,0,0,W,H);
          }
          const scale=W/900;
          const r=m.route||[],nPts=Math.floor(r.length/3);
          const partial=prog<1;
          const drawCount=partial?Math.max(1,Math.floor(nPts*prog)):nPts;
          const maxT=drawCount>0&&nPts>0?r[(drawCount-1)*3]:1e9;
          if(drawCount>=2){
            const pts=[];for(let i=0;i<drawCount;i++)pts.push(toPx(r[i*3+1],r[i*3+2]));
            ctx.save();ctx.lineCap='round';ctx.lineJoin='round';
            ctx.beginPath();ctx.moveTo(pts[0][0],pts[0][1]);for(let i=1;i<pts.length;i++)ctx.lineTo(pts[i][0],pts[i][1]);
            ctx.strokeStyle='rgba(11,11,14,0.84)';ctx.lineWidth=8*scale;ctx.stroke();
            const grad=ctx.createLinearGradient(pts[0][0],pts[0][1],pts[pts.length-1][0],pts[pts.length-1][1]);
            grad.addColorStop(0,'#30d158');grad.addColorStop(0.25,'#f59e0b');grad.addColorStop(1,'#fde68a');
            ctx.strokeStyle=grad;ctx.lineWidth=4.2*scale;ctx.shadowColor='rgba(245,158,11,0.55)';ctx.shadowBlur=10*scale;ctx.stroke();
            ctx.shadowBlur=0;
            let acc=0;
            for(let i=1;i<pts.length;i++){
              const dx=pts[i][0]-pts[i-1][0],dy=pts[i][1]-pts[i-1][1],seg=Math.hypot(dx,dy);
              acc+=seg;
              if(acc>=78*scale&&seg>4*scale){
                acc=0;const ang=Math.atan2(dy,dx),mx=(pts[i][0]+pts[i-1][0])/2,my=(pts[i][1]+pts[i-1][1])/2;
                ctx.save();ctx.translate(mx,my);ctx.rotate(ang);
                ctx.beginPath();ctx.moveTo(6*scale,0);ctx.lineTo(-4*scale,-4.5*scale);ctx.lineTo(-4*scale,4.5*scale);ctx.closePath();
                ctx.fillStyle='#ffffff';ctx.fill();ctx.restore();
              }
            }
            const [sx,sy]=pts[0],[ex,ey]=pts[pts.length-1];
            ctx.beginPath();ctx.arc(sx,sy,8*scale,0,Math.PI*2);ctx.fillStyle='#30d158';ctx.fill();ctx.lineWidth=2.5*scale;ctx.strokeStyle='#0b0b0e';ctx.stroke();
            ctx.beginPath();ctx.arc(ex,ey,8*scale,0,Math.PI*2);ctx.fillStyle=partial?'#ffffff':'#ff453a';ctx.fill();ctx.lineWidth=2.5*scale;ctx.strokeStyle='#0b0b0e';ctx.stroke();
            ctx.restore();
          }
          let shownChests=0;
          if(!withHud)rt.hitPts=[];
          if(rt.chests){
            for(const c of (m.chestPts||[])){
              if(partial&&c.t>maxT)continue;
              shownChests++;
              const [cx,cy]=toPx(c.x,c.y);
              if(!withHud)rt.hitPts.push({x:cx,y:cy,c});
              ctx.save();
              ctx.beginPath();ctx.arc(cx,cy,9.5*scale,0,Math.PI*2);
              ctx.fillStyle='#f59e0b';ctx.fill();
              ctx.lineWidth=2*scale;ctx.strokeStyle='#0b0b0e';ctx.stroke();
              ctx.fillStyle='#0b0b0e';ctx.font=`700 ${Math.max(8,Math.round(10*scale))}px -apple-system,Segoe UI,sans-serif`;ctx.textAlign='center';ctx.textBaseline='middle';
              ctx.fillText(String(c.n),cx,cy+0.5*scale);
              ctx.restore();
            }
          }else{shownChests=partial?Math.round(m.chests*prog):m.chests}
          if(withHud){
            const count=partial?shownChests:m.chests;
            const pl=m.placement?` · #${m.placement}`:'';
            const label=`${count} CHESTS`;
            const sub=`${m.mode}${pl}`;
            ctx.save();
            ctx.fillStyle='rgba(11,11,14,0.82)';
            ctx.beginPath();ctx.roundRect(12,12,174,48,10);ctx.fill();
            ctx.strokeStyle='rgba(245,158,11,0.55)';ctx.lineWidth=1.5;ctx.stroke();
            ctx.fillStyle='#f59e0b';ctx.font='700 18px -apple-system,Segoe UI,sans-serif';ctx.textBaseline='top';
            ctx.fillText(label,22,19);
            ctx.fillStyle='#d1d1d6';ctx.font='600 11px -apple-system,Segoe UI,sans-serif';
            ctx.fillText(sub,22,41);
            ctx.restore();
          }
        }
        function rtDraw(){rtDrawOn($('#rtCanvas'),rtCurMatch(),rt.playing?rt.prog:1,false)}
        // Minimal self-contained GIF89a encoder (6x6x6 RGB palette + LZW) for animated route export
        function encodeGif(w,h,frames,delays){
          const out=[];
          const wb=b=>out.push(b&255),ws=s=>{for(let i=0;i<s.length;i++)out.push(s.charCodeAt(i))},wu16=v=>{wb(v&255);wb((v>>8)&255)};
          ws('GIF89a');wu16(w);wu16(h);wb(0xF7);wb(0);wb(0);
          // 256-color Global Color Table: 6x6x6 cube (0..215) + 40 grays/gold accents (216..255)
          for(let r=0;r<6;r++)for(let g=0;g<6;g++)for(let b=0;b<6;b++){wb(r*51);wb(g*51);wb(b*51)}
          for(let i=0;i<40;i++){const v=Math.round(i*255/39);wb(v);wb(v);wb(v)}
          // NETSCAPE2.0 infinite loop
          wb(0x21);wb(0xFF);wb(11);ws('NETSCAPE2.0');wb(3);wb(1);wu16(0);wb(0);
          const idxBuf=new Uint8Array(w*h);
          for(let fi=0;fi<frames.length;fi++){
            const rgba=frames[fi];
            for(let p=0,j=0;p<idxBuf.length;p++,j+=4){
              const r=Math.round(rgba[j]/51),g=Math.round(rgba[j+1]/51),b=Math.round(rgba[j+2]/51);
              idxBuf[p]=r*36+g*6+b;
            }
            wb(0x21);wb(0xF9);wb(4);wb(0);wu16(delays[fi]||8);wb(0);wb(0);
            wb(0x2C);wu16(0);wu16(0);wu16(w);wu16(h);wb(0);
            const minCode=8,clear=256,eoi=257;
            wb(minCode);
            const sub=[];let bits=0,bitLen=0;
            const emit=(code,size)=>{bits|=(code<<bitLen);bitLen+=size;while(bitLen>=8){sub.push(bits&255);bits>>=8;bitLen-=8;if(sub.length===255){wb(255);for(let k=0;k<255;k++)wb(sub[k]);sub.length=0}}};
            let codeSize=minCode+1,nextCode=eoi+1,dict=new Int32Array(4096*256);dict.fill(-1);
            emit(clear,codeSize);
            let prefix=idxBuf[0];
            for(let p=1;p<idxBuf.length;p++){
              const c=idxBuf[p],key=(prefix<<8)|c,hit=dict[key];
              if(hit!==-1){prefix=hit}else{
                emit(prefix,codeSize);
                if(nextCode<4096){dict[key]=nextCode++;if(nextCode>(1<<codeSize)&&codeSize<12)codeSize++}
                else{emit(clear,codeSize);dict.fill(-1);codeSize=minCode+1;nextCode=eoi+1}
                prefix=c;
              }
            }
            emit(prefix,codeSize);emit(eoi,codeSize);
            if(bitLen>0)sub.push(bits&255);
            if(sub.length){wb(sub.length);for(let k=0;k<sub.length;k++)wb(sub[k])}
            wb(0);
          }
          wb(0x3B);
          return new Uint8Array(out);
        }
        async function rtExportGif(){
          const m=rtCurMatch(),btn=$('#rtGifBtn');if(!m||!btn||btn.disabled)return;
          btn.disabled=true;btn.textContent='Encoding GIF…';
          await new Promise(r=>setTimeout(r,30));
          try{
            const S=520,off=document.createElement('canvas');off.width=S;off.height=S;
            const octx=off.getContext('2d',{willReadFrequently:true});
            const nFrames=44,frames=[],delays=[];
            for(let i=0;i<=nFrames;i++){
              const p=i/nFrames;
              rtDrawOn(off,m,p,true);
              frames.push(octx.getImageData(0,0,S,S).data);
              delays.push(i===nFrames?220:8);
            }
            const gifBytes=encodeGif(S,S,frames,delays);
            const fname=`chest-route-${m.chests}-chests.gif`;
            const res=await fetch('/api/save-gif?name='+encodeURIComponent(fname),{method:'POST',headers:{'X-Token':TOKEN,'Content-Type':'image/gif'},body:gifBytes});
            const j=await res.json();
            btn.textContent=j&&j.ok?'Saved to Downloads!':'Export failed';
          }catch(e){
            btn.textContent='Export failed';
          }finally{
            setTimeout(()=>{btn.disabled=false;btn.textContent='Export GIF'},2500);
          }
        }
        function rtStopPlay(){rt.playing=false;rt.prog=1;if(rt.animId){cancelAnimationFrame(rt.animId);rt.animId=null}$('#rtPlayBtn').textContent='▶ Replay';rtDraw()}
        function rtTogglePlay(){
          if(rt.playing){rtStopPlay();return}
          rt.playing=true;rt.prog=0;$('#rtPlayBtn').textContent='■ Stop';
          const t0=performance.now(),dur=6500;
          const step=now=>{if(!rt.playing)return;rt.prog=Math.min(1,(now-t0)/dur);rtDraw();if(rt.prog<1)rt.animId=requestAnimationFrame(step);else rtStopPlay()};
          rt.animId=requestAnimationFrame(step);
        }
        function rtRenderStats(m){
          const mins=m.duration>0?m.duration/60:0,cpm=mins>0?(m.chests/mins).toFixed(2):'—';
          const pl=m.placement?`#${m.placement}`:'—';
          $('#rtStats').innerHTML=`
            <div class="rt-stat" style="border-color:rgba(245,158,11,.35)"><div class="v">${esc(m.chests)}</div><div class="l">Chests opened</div></div>
            <div class="rt-stat"><div class="v">${esc(cpm)}</div><div class="l">Chests / min</div></div>
            <div class="rt-stat"><div class="v">${esc(pl)}</div><div class="l">Placement</div></div>
            <div class="rt-stat"><div class="v">${esc(rtFmtSec(m.duration))}</div><div class="l">Duration</div></div>
            <div class="rt-stat"><div class="v" style="font-size:16px;padding-top:4px">${esc(m.mode)}</div><div class="l">${esc(m.when)}</div></div>`;
        }
        function showRoute(){
          const pane=$('#routePane');pane.classList.toggle('hidden',tab!=='route');
          if(tab!=='route'||!S)return;
          if(!rt.init){
            rt.init=true;
            rt.mapPoi=new Image();rt.mapPoi.onload=rtDraw;rt.mapPoi.src='/api/map?pois=1&t='+TOKEN;
            rt.mapBlank=new Image();rt.mapBlank.onload=rtDraw;rt.mapBlank.src='/api/map?pois=0&t='+TOKEN;
            $('#rtSelect').addEventListener('change',e=>{rt.selectedId=e.target.value;rt.customVp=null;rtStopPlay();const m=rtCurMatch();if(m){rtRenderStats(m);rtUpdateZoomLbl();rtDraw()}});
            $('#rtZoomIsland').addEventListener('click',()=>{rt.zoom='island';rt.customVp=null;$('#rtZoomIsland').classList.add('on');$('#rtZoomFit').classList.remove('on');rtUpdateZoomLbl();rtDraw()});
            $('#rtZoomFit').addEventListener('click',()=>{rt.zoom='fit';rt.customVp=null;$('#rtZoomFit').classList.add('on');$('#rtZoomIsland').classList.remove('on');rtUpdateZoomLbl();rtDraw()});
            $('#rtZoomIn').addEventListener('click',()=>rtZoomBy(0.72,0.5,0.5));
            $('#rtZoomOut').addEventListener('click',()=>rtZoomBy(1.38,0.5,0.5));
            $('#rtPoisBtn').addEventListener('click',()=>{rt.pois=!rt.pois;$('#rtPoisBtn').textContent=rt.pois?'POIs: On':'POIs: Off';$('#rtPoisBtn').classList.toggle('live-on',rt.pois);rtDraw()});
            $('#rtChestsBtn').addEventListener('click',()=>{rt.chests=!rt.chests;$('#rtChestsBtn').textContent=rt.chests?'Chests: On':'Chests: Off';$('#rtChestsBtn').classList.toggle('live-on',rt.chests);rtDraw()});
            $('#rtPlayBtn').addEventListener('click',rtTogglePlay);
            $('#rtGifBtn').addEventListener('click',rtExportGif);
            const cv=$('#rtCanvas'),tip=$('#rtTip');
            cv.addEventListener('wheel',e=>{
              e.preventDefault();
              const rect=cv.getBoundingClientRect();
              const fx=Math.max(0,Math.min(1,(e.clientX-rect.left)/rect.width));
              const fy=Math.max(0,Math.min(1,(e.clientY-rect.top)/rect.height));
              rtZoomBy(e.deltaY<0?0.82:1.22,fx,fy);
            },{passive:false});
            cv.addEventListener('dblclick',e=>{
              const rect=cv.getBoundingClientRect();
              rtZoomBy(0.58,(e.clientX-rect.left)/rect.width,(e.clientY-rect.top)/rect.height);
            });
            cv.addEventListener('pointerdown',e=>{
              if(e.button!==0)return;
              const m=rtCurMatch();if(!m)return;
              const vp=rtViewport(m);
              rt.drag={x:e.clientX,y:e.clientY,u0:vp.u0,v0:vp.v0,span:vp.span};
              cv.classList.add('drag');cv.setPointerCapture(e.pointerId);tip.classList.add('hidden');
            });
            cv.addEventListener('pointermove',e=>{
              if(rt.drag){
                const rect=cv.getBoundingClientRect();
                const du=(e.clientX-rt.drag.x)/rect.width*rt.drag.span;
                const dv=(e.clientY-rt.drag.y)/rect.height*rt.drag.span;
                rt.customVp=rtClampVp(rt.drag.u0-du,rt.drag.v0-dv,rt.drag.span);
                rtDraw();return;
              }
              const rect=cv.getBoundingClientRect(),sx=cv.width/rect.width,sy=cv.height/rect.height;
              const mx=(e.clientX-rect.left)*sx,my=(e.clientY-rect.top)*sy;
              let best=null,bd=18;
              for(const h of rt.hitPts){const d=Math.hypot(h.x-mx,h.y-my);if(d<bd){bd=d;best=h}}
              if(best){
                tip.textContent=`#${best.c.n} · ${best.c.kind}${best.c.destroyed?' (destroyed)':''} · ${rtFmtSec(best.c.t)}`;
                tip.style.left=(best.x/sx)+'px';tip.style.top=(best.y/sy)+'px';tip.classList.remove('hidden');
              }else tip.classList.add('hidden');
            });
            const endDrag=()=>{if(rt.drag){rt.drag=null;cv.classList.remove('drag')}};
            cv.addEventListener('pointerup',endDrag);cv.addEventListener('pointercancel',endDrag);
            cv.addEventListener('mouseleave',()=>tip.classList.add('hidden'));
          }
          const list=S.bestRoutes||[];
          $('#rtEmpty').classList.toggle('hidden',list.length>0);
          $('#rtWrap').classList.toggle('hidden',list.length===0);
          $('#rtStats').classList.toggle('hidden',list.length===0);
          if(!list.length)return;
          const sig=list.map(m=>m.id+':'+m.chests).join('|');
          if(sig!==rt.listSig){
            rt.listSig=sig;
            if(!rt.selectedId||!list.some(m=>m.id===rt.selectedId))rt.selectedId=list[0].id;
            $('#rtSelect').innerHTML=list.map((m,i)=>`<option value="${esc(m.id)}" ${m.id===rt.selectedId?'selected':''}>#${i+1} Best · ${esc(m.chests)} chests · ${esc(m.when)} · ${esc(m.mode)}</option>`).join('');
          }
          const cur=rtCurMatch();if(cur){rtRenderStats(cur);rtUpdateZoomLbl();if(!rt.playing)rtDraw()}
        }
        refresh(true);
        setInterval(()=>{if(!document.hidden)refresh(false)},2000);
        document.addEventListener('visibilitychange',()=>{if(!document.hidden)refresh(false)});
        </script></body></html>
        """;
}

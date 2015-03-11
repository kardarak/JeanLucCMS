# JeanLucCMS
Multi lingual CMS based on OWIN

** Theses notes are only in french for now. I will translate them when I have the time

Le but:CMS multi-langue et developper friendly (Techno: OWIN + WebApi)

La base:
  JeanLucCMS = le code du CMS
  Kardarak = le code exemple d'une siteweb (très basic pour l'instant)

Pour commencer:
   - Créer la BD (Create Db.sql) et ajuster la connection entity dans le web.config
   - Utiliser Create Test Data.sql et/ou:
     - Aller sur "/admin", puis dans "Page Types" pour créer un dossier et type de page
     - Après vous pouvez aller dans Pages pour créer la page root

Concept:
   - On cré d'abord un modèle dans Kardarak.Models.Cms (namespace configurable)
   - On cré aussi des controllers/actions (voir Kardarak.Controllers)
   - On utilise des vues Razor
   - L'outil d'admin ("/admin" = configurable) va afficher les éditeurs tel que configuré dans le modèle sélectionné dans "Page types"
   - J'utilise WebAPI au lieu de MVC pour pouvoir rouler le tout dans OWIN

Features à venir:
   - Passer à MVC6 au lieu de faire du MVC avec WebAPI2 (Utilisé pour support OWIN)
   - Ajuster l'API pour facilement monter dynamiquement un menu basé sur les pages (il faut ajouter un lien enfant-parent)
   - Créer un exemple de navigation + changement de langue
   - Editeur pour les champs "liste" dans le modèle
   - Gestionaire de médias 
   - Modifier l'éditeur HTML pour lien à l'éditeur de média
   - Listes de contenus basé sur un modèle
   - Sécurité (Accès à l'admin)
   - Champs pour ajouter un lien (vers contenu ou page)
   - CSS (J'ai utilisé bootstrap, mais le UI est loins d'être au point)
   - CodeClean
   - etc.

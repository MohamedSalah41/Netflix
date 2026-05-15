# Controller Routes — http://localhost:5155

---

## Actor

- `GET` [/Actor/GetActorByName?name=](http://localhost:5155/Actor/GetActorByName?name=) — search actor by name
- `GET` [/Actor/GetActorByID?id=](http://localhost:5155/Actor/GetActorByID?id=) — get actor by ID
- `GET` [/Actor/GetAllActors](http://localhost:5155/Actor/GetAllActors) — list all actors
- `GET` [/Actor/AddActor](http://localhost:5155/Actor/AddActor) *(Admin)* — add actor form
- `POST` `/Actor/AddActor` *(Admin)* — submit new actor
- `GET` [/Actor/UpdateActor?id=](http://localhost:5155/Actor/UpdateActor?id=) *(Admin)* — edit actor form
- `POST` `/Actor/UpdateActor/{id}` *(Admin)* — submit actor update
- `GET` [/Actor/DeleteActor?id=](http://localhost:5155/Actor/DeleteActor?id=) *(Admin)* — delete confirmation
- `POST` `/Actor/DeleteActor/{id}` *(Admin)* — confirm delete

---

## Category

- `GET` [/Category](http://localhost:5155/Category) — list all categories
- `GET` [/Category/Add](http://localhost:5155/Category/Add) *(Admin)* — add category form
- `POST` `/Category/Add` *(Admin)* — submit new category
- `GET` [/Category/Update/{id}](http://localhost:5155/Category/Update/1) *(Admin)* — edit category form
- `POST` `/Category/Update/{id}` *(Admin)* — submit category update
- `GET` [/Category/Delete/{id}](http://localhost:5155/Category/Delete/1) *(Admin)* — delete confirmation
- `POST` `/Category/Delete/{id}` *(Admin)* — confirm delete

---

## Episode

- `GET` [/Episode/GetAllEpisodes](http://localhost:5155/Episode/GetAllEpisodes) — list all episodes
- `GET` [/Episode/GetAllEpisodes?seasonId=](http://localhost:5155/Episode/GetAllEpisodes?seasonId=) — list episodes by season
- `GET` [/Episode/Details/{id}](http://localhost:5155/Episode/Details/1) — episode details
- `GET` [/Episode/Create](http://localhost:5155/Episode/Create) *(Admin)* — add episode form
- `POST` `/Episode/Create` *(Admin)* — submit new episode
- `GET` [/Episode/Edit/{id}](http://localhost:5155/Episode/Edit/1) *(Admin)* — edit episode form
- `POST` `/Episode/Edit/{id}` *(Admin)* — submit episode update
- `GET` [/Episode/Delete/{id}](http://localhost:5155/Episode/Delete/1) *(Admin)* — delete confirmation
- `POST` `/Episode/Delete/{id}` *(Admin)* — confirm delete

---

## History (API)

- `POST` `/api/history/update` *(Auth)* — update watch progress
- `GET` [/api/history/progress/{contentId}](http://localhost:5155/api/history/progress/1) *(Auth)* — get watch progress

---

## Home

- `GET` [/](http://localhost:5155/) — home page
- `GET` [/Home/Privacy](http://localhost:5155/Home/Privacy) *(Auth)* — privacy page
- `GET` [/Home/Error/{statusCode}](http://localhost:5155/Home/Error/404) — error page

---

## Movie

- `GET` [/Movie/GetAllMovies](http://localhost:5155/Movie/GetAllMovies) — list all movies
- `GET` [/Movie/Details/{id}](http://localhost:5155/Movie/Details/1) — movie details
- `GET` [/Movie/AddMovie](http://localhost:5155/Movie/AddMovie) *(Admin)* — add movie form
- `POST` `/Movie/AddMovie` *(Admin)* — submit new movie
- `GET` [/Movie/Edit/{id}](http://localhost:5155/Movie/Edit/1) *(Admin)* — edit movie form
- `POST` `/Movie/Edit/{id}` *(Admin)* — submit movie update
- `GET` [/Movie/Delete/{id}](http://localhost:5155/Movie/Delete/1) *(Admin)* — delete confirmation
- `POST` `/Movie/Delete/{id}` *(Admin)* — confirm delete

---

## MyList

- `GET` [/MyList](http://localhost:5155/MyList) *(Auth)* — view my list
- `POST` `/MyList/Toggle` *(Auth)* — add/remove item
- `GET` [/MyList/Status?mediaId=&mediaType=](http://localhost:5155/MyList/Status?mediaId=&mediaType=) *(Auth)* — check if item is in list

---

## Payment

- `GET` [/Payment/Plans](http://localhost:5155/Payment/Plans) — subscription plans
- `POST` `/Payment/CreateCheckoutSession` *(Auth)* — start Stripe checkout
- `GET` [/Payment/Success?session_id=](http://localhost:5155/Payment/Success?session_id=) *(Auth)* — payment success
- `GET` [/Payment/Cancel](http://localhost:5155/Payment/Cancel) *(Auth)* — payment cancelled
- `POST` `/Payment/Webhook` — Stripe webhook
- `POST` `/Payment/CancelSubscription` *(Auth)* — cancel active subscription

---

## Profiles

- `GET` [/Profiles/SelectProfile](http://localhost:5155/Profiles/SelectProfile) *(Auth)* — profile picker
- `POST` `/Profiles/SelectProfile` *(Auth)* — set active profile
- `GET` [/Profiles/ManageProfiles](http://localhost:5155/Profiles/ManageProfiles) *(Auth)* — manage profiles
- `GET` [/Profiles/AddProfile](http://localhost:5155/Profiles/AddProfile) *(Auth)* — add profile form
- `POST` `/Profiles/AddProfile` *(Auth)* — submit new profile
- `GET` [/Profiles/UpdateProfile/{id}](http://localhost:5155/Profiles/UpdateProfile/1) *(Auth)* — edit profile form
- `POST` `/Profiles/UpdateProfile/{id}` *(Auth)* — submit profile update
- `GET` [/Profiles/DeleteProfile/{id}](http://localhost:5155/Profiles/DeleteProfile/1) *(Auth)* — delete confirmation
- `POST` `/Profiles/DeleteProfile/{id}` *(Auth)* — confirm delete
- `GET` [/Profiles/GetProfileById?id=](http://localhost:5155/Profiles/GetProfileById?id=) — get profile by ID (JSON)
- `GET` [/Profiles/GetAllProfiles](http://localhost:5155/Profiles/GetAllProfiles) *(Admin)* — list all profiles

---

## Season

- `GET` [/Season/GetAllSeasons](http://localhost:5155/Season/GetAllSeasons) — list all seasons
- `GET` [/Season/GetSeasonById?id=](http://localhost:5155/Season/GetSeasonById?id=) — season details
- `GET` [/Season/AddSeason](http://localhost:5155/Season/AddSeason) *(Admin)* — add season form
- `POST` `/Season/AddSeason` *(Admin)* — submit new season
- `GET` [/Season/UpdateSeason/{id}](http://localhost:5155/Season/UpdateSeason/1) *(Admin)* — edit season form
- `POST` `/Season/UpdateSeason/{id}` *(Admin)* — submit season update
- `GET` [/Season/DeleteSeason/{id}](http://localhost:5155/Season/DeleteSeason/1) *(Admin)* — delete confirmation
- `POST` `/Season/DeleteSeason/{id}` *(Admin)* — confirm delete

---

## Series

- `GET` [/Series/GetAllSeries](http://localhost:5155/Series/GetAllSeries) — list all series
- `GET` [/Series/GetSeriesById?id=](http://localhost:5155/Series/GetSeriesById?id=) — series details
- `GET` [/Series/AddSeries](http://localhost:5155/Series/AddSeries) *(Admin)* — add series form
- `POST` `/Series/AddSeries` *(Admin)* — submit new series
- `GET` [/Series/UpdateSeries/{id}](http://localhost:5155/Series/UpdateSeries/1) *(Admin)* — edit series form
- `POST` `/Series/UpdateSeries/{id}` *(Admin)* — submit series update
- `GET` [/Series/DeleteSeries?id=](http://localhost:5155/Series/DeleteSeries?id=) *(Admin)* — delete confirmation
- `POST` `/Series/DeleteSeries/{id}` *(Admin)* — confirm delete

---

## Users

- `GET` [/Users](http://localhost:5155/Users) *(Admin)* — list all users
- `GET` [/Users/Edit/{id}](http://localhost:5155/Users/Edit/1) *(Admin)* — edit user form
- `POST` `/Users/Edit/{id}` *(Admin)* — submit user update
- `GET` [/Users/Delete/{id}](http://localhost:5155/Users/Delete/1) *(Admin)* — delete confirmation
- `POST` `/Users/Delete/{id}` *(Admin)* — confirm delete

---

## Identity (built-in)

- `GET` [/Identity/Account/Login](http://localhost:5155/Identity/Account/Login) — login page
- `GET` [/Identity/Account/Register](http://localhost:5155/Identity/Account/Register) — register page
- `GET` [/Identity/Account/Logout](http://localhost:5155/Identity/Account/Logout) — logout
- `GET` [/Identity/Account/ExternalLogin](http://localhost:5155/Identity/Account/ExternalLogin) — external login
- `GET` [/Identity/Account/AccessDenied](http://localhost:5155/Identity/Account/AccessDenied) — access denied
- `GET` [/Identity/Account/Manage](http://localhost:5155/Identity/Account/Manage) — account management

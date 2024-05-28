package bapsy;

import static io.gatling.javaapi.core.CoreDsl.*;
import static io.gatling.javaapi.http.HttpDsl.*;

import io.gatling.javaapi.core.*;
import io.gatling.javaapi.http.*;

import java.util.concurrent.ThreadLocalRandom;

public class WebSimulation extends Simulation {
    private HttpProtocolBuilder httpProtocol = http
        .baseUrl("http://localhost:5062/")
        .acceptHeader("application/json")
        .contentTypeHeader("application/json");

    private static ChainBuilder authenticate =
        exec(http("Authenticate")
                .post("/auth/dev")
                .body(StringBody("{\"email\": \"test@example.com\", \"name\": \"Test\"}"))
                .check(jmesPath("token").saveAs("token")));

    private static ChainBuilder getPlans =
        exec(http("Get all plans")
                .get("/bookplan/list")
                .header("Authorization", "Bearer #{token}"));

    private static ChainBuilder addPlan =
        exec(http("Add plan")
                .post("/bookplan/add")
                .header("Authorization", "Bearer #{token}")
                .body(StringBody("{\"title\": \"Title\", \"author\": \"Author\", \"cover\": \"_none\", \"pages\": 200, \"deadline\": \"2024-06-30\", \"weekdays\": [true, false, true, false, false, false, false], \"timeofday\": \"11:00\"}"))
                .check(jmesPath("id").saveAs("planId")));

    private static ChainBuilder getPlan =
        exec(http("Get plan")
                .get("/bookplan/get/#{planId}")
                .header("Authorization", "Bearer #{token}"));

    private static ChainBuilder removePlan =
        exec(http("Delete plan")
                .post("/bookplan/remove")
                .header("Authorization", "Bearer #{token}")
                .body(StringBody("{\"id\": #{planId}}")));

    private ScenarioBuilder scn = scenario("BookQuest load test")
        .exec(authenticate)
        .pause(1)
        .exec(getPlans)
        .pause(1)
        .exec(addPlan)
        .pause(1)
        .exec(getPlan)
        .pause(1)
        .exec(removePlan);

    {
        setUp(
                scn.injectOpen(atOnceUsers(1))
        ).protocols(httpProtocol);
    }

    /*FeederBuilder<String> feeder = csv("search.csv").random();

    ChainBuilder search = exec(
        http("Home").get("/"),
        pause(1),
        feed(feeder),
        http("Search")
            .get("/computers?f=#{searchCriterion}")
            .check(
                css("a:contains('#{searchComputerName}')", "href").saveAs("computerUrl")
            ),
        pause(1),
        http("Select")
            .get("#{computerUrl}")
            .check(status().is(200)),
        pause(1)
    ); */

    //// repeat is a loop resolved at RUNTIME
    //ChainBuilder browse =
    //    // Note how we force the counter name, so we can reuse it
    //    repeat(4, "i").on(
    //        http("Page #{i}").get("/computers?p=#{i}"),
    //        pause(1)
    //    );

    //// Note we should be using a feeder here
    //// let's demonstrate how we can retry: let's make the request fail randomly and retry a given
    //// number of times

    //ChainBuilder edit =
    //    // let's try at max 2 times
    //    tryMax(2)
    //        .on(
    //            http("Form").get("/computers/new"),
    //            pause(1),
    //            http("Post")
    //                .post("/computers")
    //                .formParam("name", "Beautiful Computer")
    //                .formParam("introduced", "2012-05-30")
    //                .formParam("discontinued", "")
    //                .formParam("company", "37")
    //                .check(
    //                    status().is(
    //                        // we do a check on a condition that's been customized with
    //                        // a lambda. It will be evaluated every time a user executes
    //                        // the request
    //                        session -> 200 + ThreadLocalRandom.current().nextInt(2)
    //                    )
    //                )
    //        )
    //        // if the chain didn't finally succeed, have the user exit the whole scenario
    //        .exitHereIfFailed();

    //HttpProtocolBuilder httpProtocol =
    //    http.baseUrl("https://computer-database.gatling.io")
    //        .acceptHeader("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
    //        .acceptLanguageHeader("en-US,en;q=0.5")
    //        .acceptEncodingHeader("gzip, deflate")
    //        .userAgentHeader(
    //            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:109.0) Gecko/20100101 Firefox/119.0"
    //        );

    //ScenarioBuilder users = scenario("Users").exec(search, browse);
    //ScenarioBuilder admins = scenario("Admins").exec(search, browse, edit);

    //{
    //    setUp(
    //        users.injectOpen(rampUsers(10).during(10)),
    //        admins.injectOpen(rampUsers(2).during(10))
    //    ).protocols(httpProtocol);
    //}
}

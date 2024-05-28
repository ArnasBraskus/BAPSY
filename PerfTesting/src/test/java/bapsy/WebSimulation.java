package bapsy;

import static io.gatling.javaapi.core.CoreDsl.*;
import static io.gatling.javaapi.http.HttpDsl.*;

import io.gatling.javaapi.core.*;
import io.gatling.javaapi.http.*;

public class WebSimulation extends Simulation {
    private HttpProtocolBuilder httpProtocol = http
        .baseUrl("http://localhost:5062/")
        .acceptHeader("application/json")
        .contentTypeHeader("application/json");

    private static FeederBuilder.FileBased<Object> jsonUserFeeder = jsonFile("users.json").random();
    private static FeederBuilder.FileBased<Object> jsonPlanFeeder = jsonFile("plans.json").random();

    private static ChainBuilder authenticate =
        feed(jsonUserFeeder)
        .exec(http("Authenticate")
                .post("/auth/dev")
                .body(StringBody("{\"email\": \"#{email}\", \"name\": \"#{name}\"}"))
                .check(jmesPath("token").saveAs("token")));

    private static ChainBuilder getPlans =
        exec(http("Get all plans")
                .get("/bookplan/list")
                .header("Authorization", "Bearer #{token}"));

    private static ChainBuilder addPlan =
        feed(jsonPlanFeeder)
        .exec(http("Add plan")
                .post("/bookplan/add")
                .body(StringBody("{\"title\": \"#{title}\", \"author\": \"#{author}\", \"cover\": \"#{cover}\", \"pages\": #{pages}, \"deadline\": \"#{deadline}\", \"weekdays\": #{weekdays}, \"timeofday\": \"#{timeofday}\"}"))
                .header("Authorization", "Bearer #{token}")
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
        .pause(2)
        .exec(getPlans)
        .pause(1)
        .exec(getPlan)
        .pause(2)
        .exec(removePlan);

    {
        setUp(
                scn.injectOpen(rampUsers(30).during(30))
        ).protocols(httpProtocol);
    }
}

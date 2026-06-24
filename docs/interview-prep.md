# Interview Prep

## Short Story

If I were explaining this in an interview, I’d say it like this:

I built a lightweight MES-style smart factory platform for a PCB line. The goal was to show that I can design something that feels like a real manufacturing system, not just a CRUD app. The simulator produces shop-floor events, the backend validates and stores them, Kafka carries the event stream, and the dashboard turns that stream into something an operations team can actually read.

I chose this shape on purpose. In a manufacturing environment, data usually does not arrive as one neat request from the browser. It comes from machines, brokers, schedulers, and other systems at different speeds. So I wanted the architecture to reflect that reality. The simulator stands in for line equipment, Kafka gives me an event backbone, Postgres gives me traceability, and the React dashboard gives me the live operations view.

## Why I Chose This Approach

The main design choice was to use an event-driven pipeline instead of making the frontend talk directly to a database or a tightly coupled API layer.

Why not direct UI-to-API only? Because it hides the real manufacturing problem. A smart factory system needs to handle independent producers, asynchronous events, retries, partial failures, and historical traceability. If I only built a simple request/response path, I would not be showing the kind of thinking that matters in industrial software.

Why Kafka instead of a simple in-memory queue or direct HTTP calls? Because Kafka makes the event stream durable and decoupled. That matters when multiple consumers might care about the same machine event later: dashboards, alerting, analytics, reporting, or downstream services. It also makes the system easier to explain in an interview, because it mirrors how real integration layers work in manufacturing and IIoT environments.

Why Postgres? Because the business value here is not just live status. It is traceability. A plant manager wants to know what happened to a part, which work order it belonged to, where scrap was introduced, and how downtime affected output. Postgres is the right fit for that kind of relational, queryable history.

Why a simulator? Because for a portfolio project, I wanted to prove the full system behavior without depending on physical hardware. The simulator is not pretending to be a PLC or a real machine controller; it is a controlled event generator so I can demonstrate the architecture, the data flow, and the operational dashboard in a repeatable way.

## Conclusion

The end result is a portfolio project that shows more than feature delivery. It shows that I can think in terms of systems: event flow, data ownership, operational visibility, and tradeoffs between coupling and flexibility.

If I were closing the story in an interview, I’d say:

"I intentionally designed the system around the realities of factory software. I used Kafka for decoupled event flow, Postgres for traceable history, and a simulator so the whole pipeline could be demonstrated without specialized hardware. The result is not just a dashboard; it is a small but realistic manufacturing integration platform."

## Demo Script

1. Start the stack with `docker compose up -d --build`.
2. Open the frontend at `http://localhost:3000`.
3. Open Kafka UI at `http://localhost:8081`.
4. Show machine status changes and recent production events.
5. Trigger a traceability lookup for a part ID.
6. Explain how downtime and scrap affect OEE.

## Q&A

### Why did you choose Kafka?

I chose Kafka because I wanted the design to match the real shape of industrial event systems. It lets producers and consumers stay decoupled, it supports durable event flow, and it gives room for future consumers like alerts, analytics, or reporting without changing the simulator or UI.

### Why not just use REST between the simulator and backend?

REST would work for a toy demo, but it would flatten the architecture. In a real plant, events are often generated asynchronously and need to be observed by multiple systems. Kafka makes that event flow explicit and easier to extend.

### Why did you use Postgres instead of a document database?

Because the core questions in this domain are relational: which work order was this part on, which machine processed it, what was the downtime reason, and how did that affect OEE? A relational database is a strong fit for traceability and reporting.

### How does the simulator differ from real machine integration?

It is a deterministic event generator, not a hardware interface. It stands in for shop-floor equipment so I can demonstrate the system end to end. In production, the source could be PLCs, MQTT devices, MES integrations, or industrial gateways.

### What would you improve for a real rollout?

I would add authentication, authorization, better observability, long-term time-series reporting, alerting, and more realistic machine telemetry. I would also separate read models for analytics and add stronger validation around event ordering and idempotency.

### How do you explain the value to a manufacturing team?

I would say it gives them a live view of line health, a traceable event history, and a foundation for OEE and downtime analysis. It helps operations see what is happening now, and helps engineering understand why performance changed over time.

### How would you secure the API and data pipeline?

I would add auth at the API layer, use service-to-service credentials for broker access, restrict topics and database access by role, and make sure the backend validates event payloads before persistence.

## Strong Follow-Up Topics

- Shift reporting
- MTBF and MTTR
- Alerting for downtime thresholds
- Role-based access control
- Machine telemetry and OEE trends over time
- Observability with Prometheus and Grafana

## Portfolio Positioning

- Emphasize system design, event flow, and manufacturing vocabulary.
- Keep the demo focused on end-to-end flow rather than raw feature count.
- Call out that the project is intentionally built for a portfolio showcase, but the patterns mirror real MES integrations.

# LandOLakesDairyDemo Project Charter

## Purpose
LandOLakesDairyDemo is a lightweight demo web application for a 15-minute SDLC showcase. The app presents a simple dairy products catalog inspired by Land O'Lakes and demonstrates how business goals, requirements, user stories, APIs, and demo flow connect across the software delivery lifecycle.

## Problem Statement
Stakeholders need a small, credible sample application that can be used to walk through core SDLC artifacts without introducing enterprise-scale complexity. The demo should feel realistic, but remain easy to explain and fast to build.

## Objectives
- Show a complete SDLC story from charter to demo-ready scope.
- Provide a browsable dairy product catalog with search and category filtering.
- Demonstrate basic admin product management.
- Expose simple product API endpoints to represent backend integration.
- Use seed data so the demo works immediately with no external dependencies.

## In Scope
- Product catalog web UI
- Browse by category
- Product search
- Product details view
- Admin add, edit, and delete product flows
- Product API endpoints
- Seed data for butter, cheese, cream, and half & half products

## Out of Scope
- Authentication and role provisioning
- Payments, checkout, or ordering
- Inventory management
- Reporting dashboards
- Complex workflows, approvals, or integrations
- Enterprise hosting, scaling, or compliance design

## Success Criteria
- A presenter can demonstrate the full app flow in 15 minutes.
- All primary features work against seeded demo data.
- SDLC artifacts are concise, understandable, and internally consistent.
- The demo clearly shows both end-user and admin capabilities.

## Key Stakeholders
- Demo Sponsor: Wants a clear SDLC showcase
- Product Manager: Defines scope and priorities
- Solution Architect: Shapes a lightweight technical approach
- Demo Audience: Reviews both business and technical outputs

## Solution Summary
Deliver a simple web application backed by product API endpoints and seeded catalog data. The solution prioritizes clarity, speed, and demo readiness over production-grade architecture.

## Risks and Mitigations
- Risk: Scope grows beyond demo needs
  Mitigation: Keep features limited to catalog, search, details, admin CRUD, and APIs.
- Risk: Demo becomes too technical or too shallow
  Mitigation: Balance business artifacts with visible product functionality.
- Risk: Environment setup slows the showcase
  Mitigation: Use seed data and simple local startup assumptions.
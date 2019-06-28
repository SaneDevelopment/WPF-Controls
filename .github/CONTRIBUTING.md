Contributing to WPF-Controls
============================

This document describes contribution guidelines that are specific to Sane Development WPF Controls Library. We try to follow [.NET CoreFX](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/contributing.md), but still on the way...

Coding Style Changes
--------------------

We intend to bring WPF-Controls into full conformance with the style guidelines described in [CoreFX Coding Style](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md). We plan to do that via refactoring. Till then please keep existing style.

Pull Requests
-------------

* **DO** submit all code changes via pull requests (PRs) rather than through a direct commit. PRs will be reviewed and potentially merged by the repo maintainers after a peer review that includes at least one maintainer.
* **DO NOT** submit "work in progress" PRs.  A PR should only be submitted when it is considered ready for review and subsequent merging by the contributor.
* **DO** give PRs short-but-descriptive names (e.g. "Improve code coverage for System.Console by 10%", not "Fix #1234")
* **DO** refer to any relevant issues, and include [keywords](https://help.github.com/articles/closing-issues-via-commit-messages/) that automatically close issues when the PR is merged.
* **DO** tag any users that should know about and/or review the change.
* **DO** ensure each commit successfully builds.  The entire PR must pass all tests in the Continuous Integration (CI) system before it'll be merged.
* **DO** address PR feedback in an additional commit(s) rather than amending the existing commits, and only rebase/squash them when necessary.  This makes it easier for reviewers to track changes.
* **DO** assume that ["Squash and Merge"](https://github.com/blog/2141-squash-your-commits) will be used to merge your commit unless you request otherwise in the PR.
* **DO NOT** fix merge conflicts using a merge commit. Prefer `git rebase`.
* **DO NOT** mix independent, unrelated changes in one PR. Separate real product/test code changes from larger code formatting/dead code removal changes. Separate unrelated fixes into separate PRs, especially if they are in different assemblies.
* **DO** include examples or unit tests for the change / new feature
* **DO** update the CHANGELOG.md file
* **DO** update the CONTRIBUTORS.md and AUTHORS.md files if you are not already listed there

Merging Pull Requests (for contributors with write access)
----------------------------------------------------------

* **DO** use ["Squash and Merge"](https://github.com/blog/2141-squash-your-commits) by default for individual contributions unless requested by the PR author.
  Do so, even if the PR contains only one commit. It creates a simpler history than "Create a Merge Commit".
  Reasons that PR authors may request "Merge and Commit" may include (but are not limited to):

  - The change is easier to understand as a series of focused commits. Each commit in the series must be buildable so as not to break `git bisect`.
  - Contributor is using an e-mail address other than the primary GitHub address and wants that preserved in the history. Contributor must be willing to squash
    the commits manually before acceptance.

Branches
--------

The repository contains two main branches:

- `master` - the release branch (stable, ready to production)  
- `develop` - the main branch with the latest development changes (pre-release)

You should base your work on the `develop` branch.

See [A successful git branching model](http://nvie.com/posts/a-successful-git-branching-model/) for more information about the branching model in use.

Create a branch for the bugfix/feature you want to work on: `git branch bugfix-some-error`

Checkout the branch: `git checkout bugfix-some-error`

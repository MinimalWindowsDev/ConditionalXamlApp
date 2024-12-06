<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
    xmlns:pp="http://schemas.example.com/preprocessor"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="pp">

    <!-- Parameter -->
    <xsl:param name="fix_issue_001" select="'False'" />

    <!-- Identity template -->
    <xsl:template match="@*|node()">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()" />
        </xsl:copy>
    </xsl:template>

    <!-- Template to process pp:If elements -->
    <xsl:template match="pp:If">
        <xsl:variable name="condition" select="@Condition" />
        <xsl:choose>
            <xsl:when test="starts-with($condition, '!')">
                <!-- Handle negative conditions (!fix_issue_001) -->
                <xsl:if test="$fix_issue_001 = 'False'">
                    <xsl:apply-templates select="node()" />
                </xsl:if>
            </xsl:when>
            <xsl:otherwise>
                <!-- Handle positive conditions (fix_issue_001) -->
                <xsl:if test="$fix_issue_001 = 'True'">
                    <xsl:apply-templates select="node()" />
                </xsl:if>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

</xsl:stylesheet>
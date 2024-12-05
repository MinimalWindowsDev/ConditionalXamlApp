<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:pp="http://schemas.example.com/preprocessor"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    exclude-result-prefixes="pp">

	<!-- Parameters -->
	<xsl:param name="DefineConstants" select="''" />

	<!-- Identity template -->
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()" />
		</xsl:copy>
	</xsl:template>

	<!-- Template to process pp:If elements -->
	<xsl:template match="pp:If">
		<xsl:variable name="condition" select="@Condition" />
		<xsl:variable name="constants" select="concat(';', $DefineConstants, ';')" />
		<xsl:choose>
			<xsl:when test="starts-with($condition, '!')">
				<xsl:variable name="constName" select="substring($condition, 2)" />
				<xsl:variable name="constSearch" select="concat(';', $constName, ';')" />
				<xsl:if test="not(contains($constants, $constSearch))">
					<xsl:apply-templates select="node()" />
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<xsl:variable name="constSearch" select="concat(';', $condition, ';')" />
				<xsl:if test="contains($constants, $constSearch)">
					<xsl:apply-templates select="node()" />
				</xsl:if>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

</xsl:stylesheet>
